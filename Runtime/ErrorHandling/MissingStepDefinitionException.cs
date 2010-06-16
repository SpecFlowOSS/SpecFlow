using System;
using System.Linq;
using System.Runtime.Serialization;

// the exceptions are part of the public API, keep them in TechTalk.SpecFlow namespace
namespace TechTalk.SpecFlow
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class MissingStepDefinitionException : SpecFlowException
    {
        public MissingStepDefinitionException()
            : base("No matching step definition found for one or more steps.")
        {
        }

#if !SILVERLIGHT
        protected MissingStepDefinitionException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
#endif

    }
}