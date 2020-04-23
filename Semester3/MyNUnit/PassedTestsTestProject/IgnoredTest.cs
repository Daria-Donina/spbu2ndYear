using MyNUnit.Attributes;
using System.Threading;

namespace PassedTestsTestProject
{
    public class IgnoredTest
    {
        [Test(Ignore = "This test in not completed yet.")]
        public void TestIgnored() => Thread.Sleep(200);
    }
}
