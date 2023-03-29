using System;
using System.Runtime.Serialization;

// the exceptions are part of the public API, keep them in TechTalk.SpecFlow namespace
namespace TechTalk.SpecFlow
{
    [Serializable]
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

        protected SpecFlowException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}