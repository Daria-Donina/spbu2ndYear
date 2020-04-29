using MyNUnit.Attributes;
using System;

namespace NonStaticBeforeClassTestProject
{
    public class NonStaticBeforeClass
    {
        [BeforeClass]
        public void NonStaticMethod() { }

        [Test]
        public void Test() { }
    }
}
