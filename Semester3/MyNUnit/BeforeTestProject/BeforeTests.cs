using MyNUnit.Attributes;
using System.Threading;

namespace BeforeTestProject
{
    public class BeforeTests
    {
        private bool IsBeforeMethod1Passed { get; set; }
        private bool IsBeforeMethod2Passed { get; set; }
        public static bool IsTestMethod1Passed { get; private set; }
        public static bool IsTestMethod2Passed { get; private set; }

        [Before]
        public void BeforeMethod1()
        {
            Thread.Sleep(200);
            IsBeforeMethod1Passed = true;
        }

        [Before]
        public void BeforeMethod2()
        {
            Thread.Sleep(400);
            IsBeforeMethod2Passed = true;
        }

        [Test]
        public void TestMethod1()
        {
            if (IsBeforeMethod1Passed && IsBeforeMethod2Passed)
            {
                Thread.Sleep(200);
                IsTestMethod1Passed = true;
            }
        }

        [Test]
        public void TestMethod2()
        {
            if (IsBeforeMethod1Passed && IsBeforeMethod2Passed)
            {
                IsTestMethod2Passed = true;
            }
        }
    }
}