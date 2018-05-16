using System;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow
{
    [Serializable]
    public class XUnitIgnoreTestException : Exception
    {
        public XUnitIgnoreTestException() : this("Test ignored")
        {
        }

        public XUnitIgnoreTestException(string message) : base(message)
        {
        }

        public XUnitIgnoreTestException(string message, Exception inner) : base(message, inner)
        {
        }

        protected XUnitIgnoreTestException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}