using NUnit.Framework;
using AfterClassTestProject;
using BeforeTestProject;
using AfterTestProject;
using System.Linq;
using System;
using System.Diagnostics;

namespace MyNUnit.Tests
{
    [TestFixture]
    public class MyNUnitTests
    {
        [Test]
        public void SimpleNUnitTest()
        {
            var testInfos = NUnit.RunTests("../../../../NUnitSimpleTest/bin/Debug/netcoreapp3.1");
            var test = testInfos.Where(testInfo => testInfo.Name == "TestMethod").FirstOrDefault();

            Assert.IsTrue(test.IsPassed);
            Assert.IsFalse(test.IsIgnored);
        }

        [Test]
        public void BeforeClassTest()
        {
            var testInfos = NUnit.RunTests("../../../../BeforeClassTestProject/bin/Debug/netcoreapp3.1");
            var test = testInfos.Where(testInfo => testInfo.Name == "TestMethod").FirstOrDefault();

            Assert.IsTrue(test.IsPassed);
        }

        [Test]
        public void AfterClassTest()
        {
            var testInfos = NUnit.RunTests("../../../../AfterClassTestProject/bin/Debug/netcoreapp3.1");
            var test1 = testInfos.Where(testInfo => testInfo.Name == "TestMethod1").FirstOrDefault();
            var test2 = testInfos.Where(testInfo => testInfo.Name == "TestMethod2").FirstOrDefault();

            Assert.IsTrue(test1.IsPassed);
            Assert.IsTrue(test2.IsPassed);
            Assert.AreEqual(1, AfterClassTests.Count);
        }

        [Test]
        public void BeforeTest()
        {
            var testInfos = NUnit.RunTests("../../../../BeforeTestProject/bin/Debug/netcoreapp3.1");
            var test1 = testInfos.Where(testInfo => testInfo.Name == "TestMethod1").FirstOrDefault();
            var test2 = testInfos.Where(testInfo => testInfo.Name == "TestMethod2").FirstOrDefault();

            Assert.IsTrue(test1.IsPassed);
            Assert.IsTrue(test2.IsPassed);
            Assert.AreEqual(4, BeforeTests.Count);
        }

        [Test]
        public void AfterTest()
        {
            var testInfos = NUnit.RunTests("../../../../AfterTestProject/bin/Debug/netcoreapp3.1");
            var test1 = testInfos.Where(testInfo => testInfo.Name == "TestMethod1").FirstOrDefault();
            var test2 = testInfos.Where(testInfo => testInfo.Name == "TestMethod2").FirstOrDefault();

            Assert.IsTrue(test1.IsPassed);
            Assert.IsTrue(test2.IsPassed);
            Assert.AreEqual(4, AfterTests.Count);
        }

        [Test]
        public void PassedTestsTest()
        {
            var time = Stopwatch.StartNew();
            var testInfos = NUnit.RunTests("../../../../PassedTestsTestProject/bin/Debug/netcoreapp3.1");
            time.Stop();

            var test1 = testInfos.Where(testInfo => testInfo.Name == "TestWithException").FirstOrDefault();
            var test2 = testInfos.Where(testInfo => testInfo.Name == "TestIgnored").FirstOrDefault();
            var test3 = testInfos.Where(testInfo => testInfo.Name == "SimpleTest1").FirstOrDefault();
            var test4 = testInfos.Where(testInfo => testInfo.Name == "SimpleTest2").FirstOrDefault();

            Assert.IsTrue(test1.IsPassed);

            Assert.IsTrue(test2.IsIgnored);
            Assert.AreEqual("This test in not completed yet.", test2.IgnoreReason);

            Assert.IsTrue(test3.IsPassed);
            Assert.IsTrue(test4.IsPassed);
            Assert.IsTrue(time.ElapsedMilliseconds < 350);
        }

        [Test]
        public void FailedTestsTest()
        {
            var testInfos = NUnit.RunTests("../../../../FailedTestsTestProject/bin/Debug/netcoreapp3.1");

            var test1 = testInfos.Where(testInfo => testInfo.Name == "AnotherTypeOfExceptionTest").FirstOrDefault();
            var test2 = testInfos.Where(testInfo => testInfo.Name == "UnexpectedExceptionTest").FirstOrDefault();
            var test3 = testInfos.Where(testInfo => testInfo.Name == "NoExceptionTest").FirstOrDefault();
            var test4 = testInfos.Where(testInfo => testInfo.Name == "NoAttributeTest").FirstOrDefault();

            Assert.IsFalse(test1.IsPassed);
            Assert.IsFalse(test2.IsPassed);
            Assert.IsFalse(test3.IsPassed);
            Assert.IsNull(test4);
        }

        [Test]
        public void NonStaticBeforeClassTest() => Assert.Throws<AggregateException>(
                () => NUnit.RunTests("../../../../NonStaticBeforeClassTestProject/bin/Debug/netcoreapp3.1"));

        [Test]
        public void AllSystemTest()
        {
            var testInfos = NUnit.RunTests("../../../../AllSystemTestProject/bin/Debug/netcoreapp3.1");

            var test1 = testInfos.Where(testInfo => testInfo.Name == "FineTest1").FirstOrDefault();
            var test2 = testInfos.Where(testInfo => testInfo.Name == "FineTest2").FirstOrDefault();
            var test3 = testInfos.Where(testInfo => testInfo.Name == "ExpTest").FirstOrDefault();
            var test4 = testInfos.Where(testInfo => testInfo.Name == "IgnoredTest").FirstOrDefault();
            var test5 = testInfos.Where(testInfo => testInfo.Name == "FailedTest1").FirstOrDefault();
            var test6 = testInfos.Where(testInfo => testInfo.Name == "FailedTest2").FirstOrDefault();

            Assert.IsTrue(test1.IsPassed);
            Assert.IsTrue(test2.IsPassed);
            Assert.IsTrue(test3.IsPassed);
            Assert.IsTrue(test4.IsIgnored);
            Assert.IsFalse(test5.IsPassed);
            Assert.IsFalse(test6.IsPassed);
        }
    }
}