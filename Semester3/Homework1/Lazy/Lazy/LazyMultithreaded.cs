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
        private Func<T> supplier;
        private bool isCalculated;
        private readonly object lockObject = new object();

        public LazyMultithreaded(Func<T> supplier) => this.supplier = supplier ?? throw new ArgumentNullException();

        /// <summary>
        /// Calculates value.
        /// </summary>
        /// <returns> Value that has been calculated.</returns>
        public T Get()
        {
            if (!Volatile.Read(ref isCalculated))
            {
                lock (lockObject)
                {
                    value = supplier();
                    supplier = null;
                    Volatile.Write(ref isCalculated, true);
                }
            }

            return value;
        }
    }
}
