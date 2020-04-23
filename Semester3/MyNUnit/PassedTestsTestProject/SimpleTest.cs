using MyNUnit.Attributes;
using System.Threading;

namespace PassedTestsTestProject
{
    public class SimpleTest
    {
        [Test]
        public void SimpleTest1() => Thread.Sleep(300);

        [Test]
        public void SimpleTest2() => Thread.Sleep(100);
    }
}
