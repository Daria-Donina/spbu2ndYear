using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyThreadPool
{
    class MyThreadPool
    {
        private List<Thread> threads;
        public MyThreadPool(int threadNumber)
        {
            if (threadNumber <= 0)
            {
                throw new ArgumentOutOfRangeException("Number of threads have to be a natural number.");
            }

            threads = new List<Thread>();
            for (int i = 0; i < threadNumber; ++i)
            {
                threads.Add(new Thread());
            }
        }

        private class MyTask<TResult> : IMyTask<TResult>
        {
            public bool IsCompleted { get; }
            public bool Result { get; }
            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier)
            {

            }
        }

        public IMyTask<TResult> QueueTask<TResult>(Func<TResult> supplier)
        {
            
        }

        public void Shutdown()
        {

        }
    }
}
