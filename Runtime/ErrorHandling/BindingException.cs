using System;
using System.Linq;
using System.Runtime.Serialization;

// the exceptions are part of the public API, keep them in TechTalk.SpecFlow namespace
namespace TechTalk.SpecFlow
{
#if !SILVERLIGHT
    [Serializable]
#endif

    public class BindingException : SpecFlowException
    {
        public BindingException()
        {
        }

        public BindingException(string message) : base(message)
        {
        }

        public BindingException(string message, Exception inner) : base(message, inner)
        {
        }

#if !SILVERLIGHT
        protected BindingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
#endif

    }
}