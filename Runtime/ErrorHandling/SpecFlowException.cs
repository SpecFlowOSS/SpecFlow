using System;
using System.Linq;
using System.Runtime.Serialization;

// the exceptions are part of the public API, keep them in TechTalk.SpecFlow namespace
namespace TechTalk.SpecFlow
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class SpecFlowException : Exception
    {
        public SpecFlowException()
        {
        }

        public SpecFlowException(string message) : base(message)
        {
        }

        public SpecFlowException(string message, Exception inner) : base(message, inner)
        {
        }

#if !SILVERLIGHT
        protected SpecFlowException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}