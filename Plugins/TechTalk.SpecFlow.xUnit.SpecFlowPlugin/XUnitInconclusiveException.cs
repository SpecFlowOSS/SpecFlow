using System;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin
{
    [Serializable]
    public class XUnitInconclusiveException : Exception
    {
        public XUnitInconclusiveException() : this("The step is inconclusive")
        {
        }

        public XUnitInconclusiveException(string message) : base(message)
        {
        }

        public XUnitInconclusiveException(string message, Exception inner) : base(message, inner)
        {
        }

        protected XUnitInconclusiveException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}