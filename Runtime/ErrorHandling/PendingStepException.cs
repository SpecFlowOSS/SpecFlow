using System;
using System.Linq;
using System.Runtime.Serialization;

// the exceptions are part of the public API, keep them in TechTalk.SpecFlow namespace
namespace TechTalk.SpecFlow
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class PendingStepException : SpecFlowException
    {
        public PendingStepException()
            : base("One or more step definitions are not implemented yet.")
        {
        }

#if !SILVERLIGHT
        protected PendingStepException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}