using System;

namespace Lazy
{
    public class LazyMultithreaded<T> : ILazy<T>
    {
        private T value;
        private readonly Func<T> supplier;
        private bool IsCalculated;
        private object lockObject = new object();

        public LazyMultithreaded(Func<T> supplier) => this.supplier = supplier ?? throw new ArgumentNullException();

        public T Get()
        {
            if (!IsCalculated)
            {
                lock (lockObject)
                {
                    value = supplier();
                    IsCalculated = true;
                }
            }

            return value;
        }
    }
}
