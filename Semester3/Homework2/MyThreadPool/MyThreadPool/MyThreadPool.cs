using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool
{
    class MyThreadPool
    {
        private List<Thread> threads;
        private ConcurrentQueue<Action> actions;
        private object invokeLocker = new object();
        private object taskLocker = new object();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public int ActiveThreads { get; private set; }
        private AutoResetEvent autoResetEvent = new AutoResetEvent(false);

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
        }

        private void PerformTasks()
        {
            while (true)
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    throw new ShutdownException();
                }

                if (actions.TryDequeue(out Action action))
                {
                    lock (invokeLocker)
                    {
                        action();
                    }
                }
                else
                {
                    autoResetEvent.WaitOne();
                }
            }
        }

        public void AddTask<TResult>(Func<TResult> supplier)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new ShutdownException();
            }

            lock (taskLocker)
            {
                var task = new MyTask<TResult>(supplier, this);
                actions.Enqueue(task.Execute);
                autoResetEvent.Set();
            }
        }

        public void Shutdown()
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new ShutdownException();
            }

            cancellationTokenSource.Cancel();
        }

        private class MyTask<TResult> : IMyTask<TResult>
        {
            public bool IsCompleted { get; private set; }
            private Func<TResult> supplier;
            private AggregateException exception;
            private ManualResetEvent isCalculatedResetEvent = new ManualResetEvent(false);
            private TResult result;
            private MyThreadPool threadPool;
            private Queue<Action> tasksQueue = new Queue<Action>();
            private object executeLocker = new object();

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

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier)
            {
                var newTask = new MyTask<TNewResult>(() => supplier(result), threadPool);

                if (!IsCompleted)
                {
                    tasksQueue.Enqueue(newTask.Execute);
                    return newTask;
                }


                return newTask;
            }

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
                    lock (executeLocker)
                    {
                        IsCompleted = true;
                        supplier = null;

                        isCalculatedResetEvent.Set();
                    }
                }
            }
        }
    }
}
