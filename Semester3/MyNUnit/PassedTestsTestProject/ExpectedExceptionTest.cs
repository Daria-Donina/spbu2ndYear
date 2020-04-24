using MyNUnit.Attributes;
using System;
using System.Threading;

namespace PassedTestsTestProject
{
    public class ExpectedExceptionTest
    {
        [Test(Expected = typeof(InvalidOperationException))]
        public void TestWithException()
        {
            Thread.Sleep(500);
            throw new InvalidOperationException();
        }
    }
}
