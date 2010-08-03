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
                    string[] errorMessages = ErrorDetails.Select(ed => 
                        string.Format("({0},{1}): {2}", ed.ForcedLine, ed.ForcedColumn, ed.Message)).ToArray();
                    return string.Format("{0}{1}{2}",
                                         base.Message,
                                         Environment.NewLine,
                                         string.Join(Environment.NewLine, errorMessages));
                }

                return base.Message;
            }
        }

        public const string DefaultMessage = "Invalid Gherkin file!";

        public SpecFlowParserException(List<ErrorDetail> errorDetails)
            : base(DefaultMessage)
        {
            ErrorDetails = errorDetails;
        }

        public SpecFlowParserException(ErrorDetail errorDetail)
            : base(DefaultMessage)
        {
            ErrorDetails = new List<ErrorDetail> { errorDetail };
        }

        protected SpecFlowParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}