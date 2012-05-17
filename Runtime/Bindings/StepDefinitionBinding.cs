using System;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepDefinitionBinding : MethodBinding, IStepDefinitionBinding
    {
        public StepDefinitionType StepDefinitionType { get; private set; }
        public Regex Regex { get; private set; }

        public BindingScope BindingScope { get; private set; }
        public bool IsScoped { get { return BindingScope != null; } }

#if SILVERLIGHT
        private static RegexOptions RegexOptions = RegexOptions.CultureInvariant;
#else
        private static RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
#endif

        public StepDefinitionBinding(StepDefinitionType stepDefinitionType, Regex regex, IBindingMethod bindingMethod, BindingScope bindingScope)
            : base(bindingMethod)
        {
            StepDefinitionType = stepDefinitionType;
            Regex = regex;
            BindingScope = bindingScope;
        }

        public StepDefinitionBinding(StepDefinitionType stepDefinitionType, string regexString, IBindingMethod bindingMethod, BindingScope bindingScope)
            : this(stepDefinitionType, new Regex("^" + regexString + "$", RegexOptions), bindingMethod, bindingScope)
        {
        }

        public MethodInfo MethodInfo
        {
            get { return AssertMethodInfo(); }
        }
    }
}