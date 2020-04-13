using System;

namespace MyNUnit.Attributes
{
    public class TestAttribute : Attribute
    {
        public Type Expected { get; set; }

        public string Ignore { get; set; }

        public TestAttribute() { }
    }
}