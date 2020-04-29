using MyNUnit.Attributes;
using System.Threading;

namespace AfterClassTestProject
{
    public class AfterClassTests
    {
        private static bool isTestMethod1Passed;
        private static bool isTestMethod2Passed;
        public static int Count { get; private set; }

        [AfterClass]
        public static void AfterClassMethod()
        {
            if (isTestMethod1Passed && isTestMethod2Passed)
            {
                Count++;
            }
        }

        [Test]
        public void TestMethod1()
        {
            Thread.Sleep(300);
            isTestMethod1Passed = true;
        }

        [Test]
        public void TestMethod2()
        {
            Thread.Sleep(300);
            isTestMethod2Passed = true;
        }
    }
}
