using System;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepDefinitionBinding : MethodBinding, IStepDefinitionBinding
    {
        public StepDefinitionType Type { get; private set; }
        public Regex Regex { get; private set; }

        public BindingScope BindingScope { get; private set; }
        public bool IsScoped { get { return BindingScope != null; } }

#if SILVERLIGHT
        private static RegexOptions RegexOptions = RegexOptions.CultureInvariant;
#else
        private static RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
#endif

        public StepDefinitionBinding(StepDefinitionType type, Regex regex, IBindingMethod bindingMethod, BindingScope bindingScope)
            : base(bindingMethod)
        {
            Type = type;
            Regex = regex;
            BindingScope = bindingScope;
        }

        public StepDefinitionBinding(StepDefinitionType type, string regexString, IBindingMethod bindingMethod, BindingScope bindingScope)
            : this(type, new Regex("^" + regexString + "$", RegexOptions), bindingMethod, bindingScope)
        {
        }

        public MethodInfo MethodInfo
        {
            get { return AssertMethodInfo(); }
        }
    }
}