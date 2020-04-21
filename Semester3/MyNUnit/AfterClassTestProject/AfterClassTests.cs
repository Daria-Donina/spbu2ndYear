using MyNUnit.Attributes;
using System.Threading;

namespace AfterClassTestProject
{
    public class AfterClassTests
    {
        private static bool IsTestMethod1Passed { get; set; }
        private static bool IsTestMethod2Passed { get; set; }
        public static int Count { get; private set; }

        [AfterClass]
        public static void AfterClassMethod()
        {
            if (IsTestMethod1Passed && IsTestMethod2Passed)
            {
                Count++;
            }
        }

        [Test]
        public void TestMethod1()
        {
            Thread.Sleep(300);
            IsTestMethod1Passed = true;
        }

        [Test]
        public void TestMethod2()
        {
            Thread.Sleep(300);
            IsTestMethod2Passed = true;
        }
    }
}
