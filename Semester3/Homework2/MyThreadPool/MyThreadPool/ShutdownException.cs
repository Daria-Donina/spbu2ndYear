using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyThreadPool
{

    [Serializable]
    public class ShutdownException : InvalidOperationException
    {
        public ShutdownException() { }
        public ShutdownException(string message) : base(message) { }
        public ShutdownException(string message, Exception inner) : base(message, inner) { }
        protected ShutdownException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
