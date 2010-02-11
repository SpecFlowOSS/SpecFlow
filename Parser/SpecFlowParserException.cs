using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.Parser
{
    [Serializable]
    public class SpecFlowParserException : Exception
    {
        public List<ErrorDetail> ErrorDetails { get; private set; }

        public override string Message
        {
            get
            {
                if (ErrorDetails != null)
                {
                    string[] errorMessages = ErrorDetails.Select(ed => string.Format("({0},{1}): {2}", ed.Row, ed.Column, ed.Message)).ToArray();
                    return string.Format("{0}{1}{2}",
                                         base.Message,
                                         Environment.NewLine,
                                         string.Join(Environment.NewLine, errorMessages));
                }

                return base.Message;
            }
        }

        public SpecFlowParserException()
        {
        }

        public SpecFlowParserException(string message) : base(message)
        {
        }

        public SpecFlowParserException(string message, List<ErrorDetail> errorDetails)
            : base(message)
        {
            ErrorDetails = errorDetails;
        }

        public SpecFlowParserException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SpecFlowParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}