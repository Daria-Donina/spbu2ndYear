using System;
using NUnit.Framework;
using BeforeClassTestProject;

namespace MyNUnit.Tests
{
    [TestFixture]
    public class MyNUnitTests
    {
        [Test]
        public void BeforeClassTest()
        {
            NUnit.RunTests("../../../../BeforeClassTestProject/bin/Debug/netcoreapp3.1");

            Assert.IsTrue(BeforeClassTests.IsBeforeClassCorrectMethod1Passed);
            Assert.IsTrue(BeforeClassTests.IsBeforeClassCorrectMethod2Passed);
            Assert.IsFalse(BeforeClassTests.IsNonStaticMethodPassed);
            Assert.IsFalse(BeforeClassTests.IsNonVoidMethodPassed);
            Assert.IsFalse(BeforeClassTests.IsMethodWithParametersPassed);
            Assert.IsTrue(BeforeClassTests.IsTestMethodPassed);
        }
    }
}