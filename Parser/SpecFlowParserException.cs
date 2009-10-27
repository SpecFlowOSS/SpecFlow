using System;
using System.Linq;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.Parser
{
    [Serializable]
    public class SpecFlowParserException : Exception
    {
        public string[] DetailedErrors { get; private set; }

        public override string Message
        {
            get
            {
                if (DetailedErrors != null)
                    return string.Format("{0}{1}{2}",
                                    base.Message,
                                    Environment.NewLine,
                                    string.Join(Environment.NewLine, DetailedErrors));
                return base.Message;
            }
        }

        public SpecFlowParserException()
        {
        }

        public SpecFlowParserException(string message) : base(message)
        {
        }

        public SpecFlowParserException(string message, string[] detailedErrors) : base(message)
        {
            DetailedErrors = detailedErrors;
        }

        public SpecFlowParserException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SpecFlowParserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}