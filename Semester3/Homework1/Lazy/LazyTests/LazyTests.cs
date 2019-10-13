using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Lazy.Tests
{
    [TestClass]
    public class LazyTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSupplierTest() => _ = LazyFactory<int>.CreateLazy(null);

        [TestMethod]
        public void GetTest()
        {
            var testString = "test";
            var lazy = LazyFactory<string>.CreateLazy(() => testString);

            Assert.AreEqual(testString, lazy.Get());
        }

        [TestMethod]
        public void RepeatedGetCallTest()
        {
            var lazy = LazyFactory<int[]>.CreateLazy(() => (new int[] { 1, 2, 3, 4, 5, 6 }));

            var firstResult = lazy.Get();
            Assert.AreEqual(firstResult, lazy.Get());
        }

        [TestMethod]
        public void ValueIsNotCalculatedMoreThanOnceTest()
        {
            int count = 0;
            var lazy = LazyFactory<int>.CreateLazy(() => ++count);

            const int amountOfCalls = 10;
            for (int i = 0; i < amountOfCalls; ++i)
            {
                Assert.AreEqual(1, lazy.Get());
            }
        }
    }
}
