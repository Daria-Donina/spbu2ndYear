using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool
{
    public class MyThreadPool
    {
        private List<Thread> threads;
        private ConcurrentQueue<Action> actions;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public int ActiveThreads { get; private set; }
        private AutoResetEvent taskAdded = new AutoResetEvent(false);

        //При создании объекта MyThreadPool в нем должно начать работу n потоков
        public MyThreadPool(int threadNumber)
        {
            if (threadNumber <= 0)
            {
                throw new ArgumentOutOfRangeException("Number of threads have to be a natural number.");
            }

            threads = new List<Thread>();
            actions = new ConcurrentQueue<Action>();

            for (int i = 0; i < threadNumber; ++i)
            {
                threads.Add(new Thread(PerformTasks));
                threads[i].Start();
            }

            ActiveThreads = threadNumber;
        }

        //У каждого потока есть два состояния: ожидание задачи / выполнение задачи
        //
        //При добавлении задачи, если в пуле есть ожидающий поток, то он должен приступить к ее исполнению. 
        //Иначе задача будет ожидать исполнения, пока не освободится какой-нибудь поток
        //
        //уже запущенные задачи не прерываются, 
        //но новые задачи не принимаются на исполнение потоками из пула.
        //
        //дать всем задачам, которые уже попали в очередь, досчитаться
        private void PerformTasks()
        {
            while (true)
            {
                if (actions.TryDequeue(out Action action))
                {
                    action();
                }
                else
                {
                    taskAdded.WaitOne();
                }
            }
        }

        //Задачи, принятые к исполнению, представлены в виде объектов интерфейса IMyTask<TResult>
        public IMyTask<TResult> AddTask<TResult>(Func<TResult> supplier)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new ShutdownException();
            }

            var task = new MyTask<TResult>(supplier, this);
            AddAction(task.Execute);

            return task;
        }

        private void AddAction(Action action)
        {
            actions.Enqueue(action);
            taskAdded.Set();
        }

        //Метод Shutdown должен завершить работу потоков. Завершение работы коллаборативное, 
        //с использованием CancellationToken
        public void Shutdown() => cancellationTokenSource.Cancel();

        //Задача — вычисление некоторого значения, описывается в виде Func<TResult>
        private class MyTask<TResult> : IMyTask<TResult>
        {
            public bool IsCompleted { get; private set; }
            private Func<TResult> supplier;
            private AggregateException exception;
            private ManualResetEvent isCalculatedResetEvent = new ManualResetEvent(false);
            private TResult result;
            private MyThreadPool threadPool;
            private Queue<Action> tasksQueue = new Queue<Action>();
            private object queueLock = new object();

            //Свойство Result возвращает результат выполнения задачи
            //
            //Если результат еще не вычислен, метод ожидает его и возвращает полученное значение, блокируя вызвавший его поток
            public TResult Result
            {
                get
                {
                    isCalculatedResetEvent.WaitOne();

                    if (exception != null)
                    {
                        throw exception;
                    }

                    return result;
                }
                private set
                {
                    result = value;
                }
            }

            public MyTask(Func<TResult> supplier, MyThreadPool threadPool)
            {
                this.supplier = supplier;
                this.threadPool = threadPool;
            }

            //Метод ContinueWith — принимает объект типа Func<TResult, TNewResult>, 
            //который может быть применен к результату данной задачи X и возвращает новую задачу Y, принятую к исполнению
            //
            //Новая задача будет исполнена не ранее, чем завершится исходная
            //В качестве аргумента объекту Func будет передан результат исходной задачи, 
            //и все Y должны исполняться на общих основаниях (т.е. должны разделяться между потоками пула)
            //
            //Метод ContinueWith может быть вызван несколько раз
            //
            //Метод ContinueWith не должен блокировать работу потока, если результат задачи X ещё не вычислен
            //
            //ContinueWith должен быть согласован с Shutdown --- принятая как ContinueWith задача должна либо досчитаться,
            //либо бросить исключение ожидающему её потоку.
            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier)
            {
                if (threadPool.cancellationTokenSource.IsCancellationRequested)
                {
                    throw new ShutdownException();
                }

                var newTask = new MyTask<TNewResult>(() => supplier(result), threadPool);

                if (!IsCompleted)
                {
                    lock (queueLock)
                    {
                        tasksQueue.Enqueue(newTask.Execute);
                    }
                    tasksQueue.Enqueue(newTask.Execute);
                    return newTask;
                }

                lock (queueLock)
                {
                    threadPool.AddAction(newTask.Execute);
                }
                
                return newTask;
            }

            //Свойство IsCompleted возвращает true, если задача выполнена
            //
            //В случае, если соответствующая задаче функция завершилась с исключением, 
            //этот метод должен завершиться с исключением AggregateException, содержащим внутри себя исключение, 
            //вызвавшее проблему
            public void Execute()
            {
                try
                {
                    result = supplier();
                }
                catch (Exception exception)
                {
                    this.exception = new AggregateException(exception);
                }
                finally
                {
                    IsCompleted = true;
                    supplier = null;
                    isCalculatedResetEvent.Set();

                    lock (queueLock)
                    {
                        while (tasksQueue.Count != 0)
                        {
                            threadPool.AddAction(tasksQueue.Dequeue());
                        }
                    }
                }
            }
        }
    }
}
