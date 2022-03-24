using System;
using System.Runtime.Serialization;

// the exceptions are part of the public API, keep them in TechTalk.SpecFlow namespace
namespace TechTalk.SpecFlow
{
    [Serializable]
    public class MissingStepDefinitionException : SpecFlowException
    {
        public MissingStepDefinitionException()
            : base("No matching step definition found for one or more steps.")
        {
        }

        protected MissingStepDefinitionException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}