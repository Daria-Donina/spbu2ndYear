﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyNUnit
{
    class TestAttribute : Attribute
    {
        public Type Expected { get; set; }

        public string Ignore { get; set; }

        public TestAttribute() { }
    }
}
