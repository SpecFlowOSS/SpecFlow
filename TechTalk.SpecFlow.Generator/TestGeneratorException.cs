using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TechTalk.SpecFlow.Generator
{
    [Serializable]
    public class TestGeneratorException : Exception
    {
        public TestGeneratorException()
        {
        }

        public TestGeneratorException(string message) : base(message)
        {
        }

        public TestGeneratorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TestGeneratorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
