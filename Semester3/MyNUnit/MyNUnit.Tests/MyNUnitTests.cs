using NUnit.Framework;
using BeforeClassTestProject;
using AfterClassTestProject;
using NUnitSimpleTest;
using BeforeTestProject;
using AfterTestProject;

namespace MyNUnit.Tests
{
    [TestFixture]
    public class MyNUnitTests
    {
        [Test]
        public void SimpleNUnitTest()
        {
            NUnit.RunTests("../../../../NUnitSimpleTest/bin/Debug/netcoreapp3.1");
            Assert.IsTrue(NUnitSimpleTestClass.IsTestPassed);
        }

        [Test]
        public void BeforeClassTest()
        {
            NUnit.RunTests("../../../../BeforeClassTestProject/bin/Debug/netcoreapp3.1");

            Assert.IsTrue(BeforeClassTests.IsTestMethodPassed);
        }

        [Test]
        public void AfterClassTest()
        {
            NUnit.RunTests("../../../../AfterClassTestProject/bin/Debug/netcoreapp3.1");

            Assert.AreEqual(1, AfterClassTests.Count);
        }

        [Test]
        public void BeforeTest()
        {
            NUnit.RunTests("../../../../BeforeTestProject/bin/Debug/netcoreapp3.1");

            Assert.IsTrue(BeforeTests.IsTestMethod1Passed);
            Assert.IsTrue(BeforeTests.IsTestMethod2Passed);
            Assert.AreEqual(4, BeforeTests.Count);
        }

        [Test]
        public void AfterTest()
        {
            NUnit.RunTests("../../../../AfterTestProject/bin/Debug/netcoreapp3.1");

            Assert.AreEqual(4, AfterTests.Count);
        }
    }
}