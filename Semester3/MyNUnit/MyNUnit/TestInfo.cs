using System;

namespace MyNUnit
{
    public class TestInfo
    {
        public string Name { get; private set; }
        public bool IsPassed { get; set; }
        public bool IsIgnored { get; set; }
        public string IgnoreReason { get; set; }
        public TimeSpan ExecutionTime { get; set; }

        public TestInfo(string name) => Name = name;
    }
}
