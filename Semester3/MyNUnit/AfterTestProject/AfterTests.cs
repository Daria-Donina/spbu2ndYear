using MyNUnit.Attributes;
using System.Threading;

namespace AfterTestProject
{
    public class AfterTests
    {
        private static int count;

        public static int Count { get => count; private set => count = value; }
        private bool IsTestMethod1Passed { get; set; }
        private bool IsTestMethod2Passed { get; set; }

        [After]
        public void AfterMethod1()
        {
            if (IsTestMethod1Passed || IsTestMethod2Passed)
            {
                Thread.Sleep(500);
                Interlocked.Increment(ref count);
            }
        }

        [After]
        public void AfterMethod2()
        {
            if (IsTestMethod1Passed || IsTestMethod2Passed)
            {
                Thread.Sleep(700);
                Interlocked.Increment(ref count);
            }
        }

        [Test]
        public void TestMethod1()
        {
            Thread.Sleep(200);
            IsTestMethod1Passed = true;
        }

        [Test]
        public void TestMethod2() => IsTestMethod2Passed = true;
    }
}
