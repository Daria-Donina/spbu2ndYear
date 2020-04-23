using System;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// Attribute for the method to be invoked as test.
    /// </summary>
    public class TestAttribute : Attribute
    {
        /// <summary>
        /// Type of exception expected when invoking a method with this attribute.
        /// </summary>
        public Type Expected { get; set; }

        /// <summary>
        /// A reason why a method with this attribute is ignored. If this field is not null, test is ignored. 
        /// </summary>
        public string Ignore { get; set; }

        public TestAttribute() { }
    }
}