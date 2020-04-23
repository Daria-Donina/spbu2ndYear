using MyNUnit.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FailedTestsTestProject
{
    public class FailedTests
    {
        [Test(Expected = typeof(ArgumentOutOfRangeException))]
        public void AnotherTypeOfExceptionTest() => throw new InvalidOperationException();

        [Test]
        public void UnexpectedExceptionTest() => throw new NullReferenceException();

        [Test(Expected = typeof(InvalidOperationException))]
        public void NoExceptionTest() => Thread.Sleep(100);

        public void NoAttributeTest() { }
    }
}
