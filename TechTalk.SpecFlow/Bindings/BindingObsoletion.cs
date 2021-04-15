using System;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    #nullable enable
    public readonly struct BindingObsoletion
    {
        private const string DefaultObsoletionMessage = "it is marked with ObsoleteAttribute but no custom message was provided.";

        internal static readonly BindingObsoletion NotObsolete = new BindingObsoletion();

        private readonly string? message;
        private readonly string? methodName;

        public bool IsObsolete => message is not null;

        public BindingObsoletion(IBindingMethod methodBinding, ObsoleteAttribute? attribute)
        {
            if (attribute is not null)
            {
                message = attribute.Message ?? DefaultObsoletionMessage;
                methodName = methodBinding.Name;
            }
            else
            {
                message = null;
                methodName = null;
            }
        }

        public string Message => IsObsolete ? $"The step {methodName} is obsolete because {message}" : string.Empty;
    }
}
