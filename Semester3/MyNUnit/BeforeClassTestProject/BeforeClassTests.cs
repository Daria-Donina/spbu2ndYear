using System.Threading;
using MyNUnit.Attributes;

namespace BeforeClassTestProject
{
    public class BeforeClassTests
    {
        public static bool IsBeforeClassCorrectMethod1Passed { get; private set; }
        public static bool IsBeforeClassCorrectMethod2Passed { get; private set; }

        public static bool IsTestMethodPassed { get; private set; }

        [BeforeClass]
        public static void BeforeClassCorrectMethod1()
        {
            Thread.Sleep(1000);
            IsBeforeClassCorrectMethod1Passed = true;
        }

        [BeforeClass]
        public static void BeforeClassCorrectMethod2()
        {
            Thread.Sleep(500);
            IsBeforeClassCorrectMethod2Passed = true;
        }

        [Test]
        public void TestMethod()
        {
            if (IsBeforeClassCorrectMethod1Passed && IsBeforeClassCorrectMethod2Passed)
            {
                Thread.Sleep(2000);
                IsTestMethodPassed = true;
            }
        }
    }
}
