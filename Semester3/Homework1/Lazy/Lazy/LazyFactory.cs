using System;

namespace Lazy
{
    /// <summary>
    /// Object producing lazy objects.
    /// </summary>
    /// <typeparam name="T"> Type of calculated value.</typeparam>
    public class LazyFactory<T>
    {
        /// <summary>
        /// Creates lazy calculation object.
        /// </summary>
        /// <param name="supplier"> Object returning value. </param>
        /// <returns> Value that has been calculated.</returns>
        public static Lazy<T> CreateLazy(Func<T> supplier) => new Lazy<T>(supplier);

        /// <summary>
        /// Creates thread safe lazy calculation object.
        /// </summary>
        /// <param name="supplier"> Object returning value.</param>
        /// <returns> Value that has been calculated.</returns>
        public static LazyMultithreaded<T> CreateLazyMultitreaded(Func<T> supplier) => new LazyMultithreaded<T>(supplier);
    }
}
