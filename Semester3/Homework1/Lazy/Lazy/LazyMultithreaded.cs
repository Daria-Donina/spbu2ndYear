using System;
using System.Threading;

namespace Lazy
{
    /// <summary>
    /// Class implementing thread safe lazy calculation object.
    /// </summary>
    /// <typeparam name="T"> Type of calculated value.</typeparam>
    public class LazyMultithreaded<T> : ILazy<T>
    {
        private T value;
        private readonly Func<T> supplier;
        private bool IsCalculated;
        private readonly object lockObject = new object();

        public LazyMultithreaded(Func<T> supplier) => this.supplier = supplier ?? throw new ArgumentNullException();

        /// <summary>
        /// Calculates value.
        /// </summary>
        /// <returns> Value that has been calculated.</returns>
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
