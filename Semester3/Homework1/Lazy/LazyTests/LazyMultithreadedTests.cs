using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Lazy.Tests
{
    [TestClass]
    public class LazyMultithreadedTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSupplierTest() => _ = LazyFactory<int>.CreateLazyMultitreaded(null);

        [TestMethod]
        public void GetTest()
        {
            var testString = "test";
            var lazy = LazyFactory<string>.CreateLazyMultitreaded(() => testString);

            Assert.AreEqual(testString, lazy.Get());
        }

        [TestMethod]
        public void RepeatedGetCallTest()
        {
            var lazy = LazyFactory<int[]>.CreateLazyMultitreaded(() => (new int[] { 1, 2, 3, 4, 5, 6 }));

            const int amountOfThreads = 10;
            var threads = new List<Thread>();

            for (int i = 0; i < amountOfThreads; ++i)
            {
                threads.Add(new Thread(() =>
                {
                    var firstResult = lazy.Get();
                    Assert.AreEqual(firstResult, lazy.Get());
                }));
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        [TestMethod]
        public void ValueIsNotCalculatedMoreThanOnceTest()
        {
            int count = 0;
            var lazy = LazyFactory<int>.CreateLazyMultitreaded(() => ++count);

            const int amountOfThreads = 10;
            var threads = new List<Thread>();

            for (int i = 0; i < amountOfThreads; ++i)
            {
                threads.Add(new Thread(() =>
                {
                    const int amountOfCalls = 10;
                    for (int j = 0; j < amountOfCalls; ++j)
                    {
                        Assert.AreEqual(1, lazy.Get());
                    }
                }));
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}
