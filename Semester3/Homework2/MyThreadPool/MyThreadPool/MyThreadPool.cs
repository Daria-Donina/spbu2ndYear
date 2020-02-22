using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool
{
    /// <summary>
    /// Provides a pool of threads that can be used to execute tasks.
    /// </summary>
    public class MyThreadPool
    {
        private readonly List<Thread> threads;
        private readonly ConcurrentQueue<Action> actions;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Returns a number of running threads in threadpool.
        /// </summary>
        public int ActiveThreads { get; private set; }
        private AutoResetEvent taskAdded = new AutoResetEvent(false);

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

        /// <summary>
        /// Adds task to the threadpool to be calculated.
        /// </summary>
        /// <typeparam name="TResult">Type of the task result.</typeparam>
        /// <param name="supplier">A function describing task.</param>
        /// <returns>Task created using the supplier.</returns>
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

        /// <summary>
        /// Closes the threadpool.
        /// </summary>
        public void Shutdown() => cancellationTokenSource.Cancel();

        /// <summary>
        /// Represents an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult">Type of the task result.</typeparam>
        private class MyTask<TResult> : IMyTask<TResult>
        {
            /// <summary>
            /// Reports whether the task is completed or not.
            /// </summary>
            public bool IsCompleted { get; private set; }
            private Func<TResult> supplier;
            private AggregateException exception;
            private readonly ManualResetEvent isCalculatedResetEvent = new ManualResetEvent(false);
            private TResult result;
            private readonly MyThreadPool threadPool;
            private readonly Queue<Action> tasksQueue = new Queue<Action>();
            private readonly object queueLock = new object();

            /// <summary>
            /// Returns the result of the task.
            /// </summary>
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

            /// <summary>
            /// Creates a continuation that executes when the target task completes.
            /// </summary>
            /// <typeparam name="TNewResult">Type of the continuation result.</typeparam>
            /// <param name="supplier">A function describing continuation.</param>
            /// <returns>The created continuation.</returns>
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
