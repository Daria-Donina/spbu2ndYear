using System;

namespace Lazy
{
    public class LazyFactory<T>
    {
        public static Lazy<T> CreateLazy(Func<T> supplier) => new Lazy<T>(supplier);

        public static LazyMultithreaded<T> CreateLazyMultitreaded(Func<T> supplier) => new LazyMultithreaded<T>(supplier);
    }
}
