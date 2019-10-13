using System;
using System.Threading;

namespace Lazy
{
    public class LazyMultithreaded<T> : ILazy<T>
    {
        private T value;
        private readonly Func<T> supplier;
        private bool IsCalculated;
        private readonly object lockObject = new object();

        public LazyMultithreaded(Func<T> supplier) => this.supplier = supplier ?? throw new ArgumentNullException();

        public T Get()
        {
            if (!IsCalculated)
            {
                lock (lockObject)
                {
                    value = supplier();
                    Volatile.Write(ref IsCalculated, true);
                }
            }

            return value;
        }
    }
}
