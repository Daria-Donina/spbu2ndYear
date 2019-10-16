using System;

namespace Lazy
{
    /// <summary>
    /// Class implementing lazy calculation object.
    /// </summary>
    /// <typeparam name="T"> Type of calculated value.</typeparam>
    public class Lazy<T> : ILazy<T>
    {
        private T value;
        private Func<T> supplier;
        private bool isCalculated;

        public Lazy(Func<T> supplier) => this.supplier = supplier ?? throw new ArgumentNullException();

        /// <summary>
        /// Calculates value.
        /// </summary>
        /// <returns> Value that has been calculated.</returns>
        public T Get()
        {
            if (!isCalculated)
            {
                value = supplier();
                supplier = null;
                isCalculated = true;
            }

            return value;
        }
    }
}
