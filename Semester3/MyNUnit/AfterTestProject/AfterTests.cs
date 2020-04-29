using MyNUnit.Attributes;
using System.Threading;

namespace AfterTestProject
{
    public class AfterTests
    {
        private static int count;

        public static int Count { get => count; private set => count = value; }
        private bool isTestMethod1Passed;
        private bool isTestMethod2Passed;

        [After]
        public void AfterMethod1()
        {
            if (isTestMethod1Passed || isTestMethod2Passed)
            {
                Thread.Sleep(500);
                Interlocked.Increment(ref count);
            }
        }

        [After]
        public void AfterMethod2()
        {
            if (isTestMethod1Passed || isTestMethod2Passed)
            {
                Thread.Sleep(700);
                Interlocked.Increment(ref count);
            }
        }

        [Test]
        public void TestMethod1()
        {
            Thread.Sleep(200);
            isTestMethod1Passed = true;
        }

        [Test]
        public void TestMethod2() => isTestMethod2Passed = true;
    }
}
