using MyNUnit.Attributes;
using System;
using System.Threading;

namespace AllSystemTestProject
{
    public class AllSystemsTests
    {
        [BeforeClass]
        public static void DoBeforeClass() => Thread.Sleep(100);

        [AfterClass]
        public static void DoAfterClass() => Thread.Sleep(100);

        [Before]
        public void DoBeforeTest() { }

        [After]
        public void DoAfterTest() { }

        [Test]
        public void FineTest1() { }

        [Test]
        public void FailedTest1()
        {
            Thread.Sleep(50);
            throw new InvalidOperationException();
        }

        [Test]
        public void FineTest2() => Thread.Sleep(100);

        [Test(Expected = typeof(ArgumentException))]
        public void ExpTest() => throw new ArgumentException();

        [Test(Expected = typeof(DivideByZeroException))]
        public void FailedTest2() { }

        [Test(Ignore = "Too long")]
        public void IgnoredTest()
        {
            Thread.Sleep(50000);
            throw new ArgumentOutOfRangeException();
        }
    }
}