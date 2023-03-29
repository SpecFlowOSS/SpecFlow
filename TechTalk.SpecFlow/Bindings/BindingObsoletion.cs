using System;
using System.Linq;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public class BindingObsoletion
    {
        private const string defaultObsoletionMessage = "it is marked with ObsoleteAttribute but no custom message was provided.";

        private readonly string message;
        private readonly string methodName;

        public BindingObsoletion(IStepDefinitionBinding stepBinding)
        {
            ObsoleteAttribute possibleObsoletionAttribute;
            try
            {
                possibleObsoletionAttribute = stepBinding?.Method.AssertMethodInfo()
                            .GetCustomAttributes(false).OfType<ObsoleteAttribute>()
                            .FirstOrDefault();
            }
            catch (Exception)
            {
                possibleObsoletionAttribute = null;
            }

            if (possibleObsoletionAttribute == null)
            {
                IsObsolete = false;
                message = null;
                methodName = null;
            }
            else
            {
                IsObsolete = true;
                message = possibleObsoletionAttribute.Message ?? defaultObsoletionMessage;
                methodName = stepBinding.Method.Name;
            }
        }

        public bool IsObsolete { get; private set; }

        public string Message => IsObsolete ? $"The step {methodName} is obsolete because {message}" : string.Empty;
    }
}
