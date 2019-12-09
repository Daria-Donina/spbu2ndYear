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

        private class MyTask<TResult> : IMyTask<TResult>
        {
            public bool IsCompleted { get; }
            public bool Result { get; private set; }
            private Func<TResult> supplier;
            ConcurrentQueue<Action> myTaskActions = new ConcurrentQueue<Action>();
            private TResult result;
            private object locker = new object();

            public MyTask(Func<TResult> supplier)
            {
                this.supplier = supplier;
            }

            public Action GetAction()
            {
                
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier)
            {

            }

            private void Calculate()
            {
                lock (locker)
                {
                    result = supplier();
                }
            }
        }

        private void PerformTasks()
        {
            while (true)
            {
                if (actions.TryDequeue(out Action action))
                {
                    action.Invoke();
                }
            }
        }

        private void AddTask<TResult>(Func<TResult> supplier)
        {
            var task = new MyTask<TResult>(supplier);
            actions.Enqueue(task);
        }

        public void Shutdown()
        {

        }
    }
}
