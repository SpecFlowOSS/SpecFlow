using System;
using System.Linq;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow
{
    [Serializable]
    public class BindingException : SpecFlowException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public BindingException()
        {
        }

        public BindingException(string message) : base(message)
        {
        }

        public BindingException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BindingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class SpecFlowException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public SpecFlowException()
        {
        }

        public SpecFlowException(string message) : base(message)
        {
        }

        public SpecFlowException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SpecFlowException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class MissingStepDefinitionException : SpecFlowException
    {
        public MissingStepDefinitionException()
            : base("No matching step definition found for one or more steps.")
        {
        }

        protected MissingStepDefinitionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class PendingStepException : SpecFlowException
    {
        public PendingStepException()
            : base("One or more step definitions are not implemented yet.")
        {
        }

        protected PendingStepException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}