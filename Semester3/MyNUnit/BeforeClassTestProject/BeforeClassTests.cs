using System;
using System.Threading;
using MyNUnit.Attributes;

namespace BeforeClassTestProject
{
    public class BeforeClassTests
    {
        private static bool isBeforeClassCorrectMethod1Passed;
        private static bool isBeforeClassCorrectMethod2Passed;

        [BeforeClass]
        public static void BeforeClassCorrectMethod1()
        {
            Thread.Sleep(1000);
            isBeforeClassCorrectMethod1Passed = true;
        }

        [BeforeClass]
        public static void BeforeClassCorrectMethod2()
        {
            Thread.Sleep(500);
            isBeforeClassCorrectMethod2Passed = true;
        }

        [Test]
        public void TestMethod()
        {
            if (!isBeforeClassCorrectMethod1Passed || !isBeforeClassCorrectMethod2Passed)
            {
                throw new Exception();
            }

            Thread.Sleep(2000);
        }
    }
}
