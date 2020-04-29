using System;

namespace MyNUnit
{
    /// <summary>
    /// Class providing access to the information about a test.
    /// </summary>
    public class TestInfo
    {
        /// <summary>
        /// Name of the test method.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Indicates whether the test is passed or not.
        /// </summary>
        public bool IsPassed { get; set; }

        /// <summary>
        /// Indicates whether the test is ignored or not.
        /// </summary>
        public bool IsIgnored { get; set; }

        /// <summary>
        /// The reason why the test is ignored.
        /// </summary>
        public string IgnoreReason { get; set; }

        /// <summary>
        /// Test running time.
        /// </summary>
        public TimeSpan ExecutionTime { get; set; }

        public TestInfo(string name) => Name = name;
    }
}
