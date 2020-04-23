using MyNUnit.Attributes;
using System;
using System.Threading;

namespace BeforeTestProject
{
    public class BeforeTests
    {
        private static int count;

        private bool isBeforeMethod1Passed;
        private bool isBeforeMethod2Passed;
        public static int Count { get => count; private set => count = value; }

        [Before]
        public void BeforeMethod1()
        {
            Thread.Sleep(200);
            isBeforeMethod1Passed = true;
            Interlocked.Increment(ref count);
        }

        [Before]
        public void BeforeMethod2()
        {
            Thread.Sleep(400);
            isBeforeMethod2Passed = true;
            Interlocked.Increment(ref count);
        }

        [Test]
        public void TestMethod1()
        {
            if (!isBeforeMethod1Passed || !isBeforeMethod2Passed)
            {
                throw new Exception();
            }

            Thread.Sleep(200);
        }

        [Test]
        public void TestMethod2()
        {
            if (!isBeforeMethod1Passed || !isBeforeMethod2Passed)
            {
                throw new Exception();
            }
        }
    }
}