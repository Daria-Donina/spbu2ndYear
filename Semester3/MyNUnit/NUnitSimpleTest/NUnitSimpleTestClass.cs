using System;
using System.Threading;
using MyNUnit.Attributes;

namespace NUnitSimpleTest
{
    public class NUnitSimpleTestClass
    {
        public static bool IsTestPassed { get; private set; }

        [Test]
        public void TestMethod()
        {
            Thread.Sleep(500);
            IsTestPassed = true;
        }
    }
}
