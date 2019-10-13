using System;

namespace Lazy
{
    public class Lazy<T> : ILazy<T>
    {
        private T value;
        private readonly Func<T> supplier;
        private bool IsCalculated;

        public Lazy(Func<T> supplier) => this.supplier = supplier ?? throw new ArgumentNullException();

        public T Get()
        {
            if (!IsCalculated)
            {
                value = supplier();
                IsCalculated = true;
            }

            return value;
        }
    }
}
