using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepArgumentTransformationBinding : MethodBinding, IStepArgumentTransformationBinding
    {
        public string Name { get; }

        public Regex Regex { get; }

        public StepArgumentTransformationBinding(Regex regex, IBindingMethod bindingMethod, string name = null)
            : base(bindingMethod)
        {
            Regex = regex;
            Name = name;
        }

        public StepArgumentTransformationBinding(string regexString, IBindingMethod bindingMethod, string name = null)
            : this(CreateRegexOrNull(regexString), bindingMethod, name)
        {
        }

        private static Regex CreateRegexOrNull(string regexString)
        {
            if (regexString == null)
                return null;
            return RegexFactory.CreateWholeTextRegexForBindings(regexString);
        }
    }
}