using System;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

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

        public StepDefinitionBinding(RuntimeConfiguration runtimeConfiguration, IErrorProvider errorProvider, StepDefinitionType type, string regexString, IBindingMethod bindingMethod, BindingScope bindingScope)
            : base(runtimeConfiguration, errorProvider, bindingMethod)
        {
            Type = type;
            Regex regex = new Regex("^" + regexString + "$", RegexOptions);
            Regex = regex;
            this.BindingScope = bindingScope;
        }

        public MethodInfo MethodInfo
        {
            get { return AssertMethodInfo(); }
        }

        public void Invoke(IContextManager contextManager, ITestTracer testTracer, object[] arguments, out TimeSpan duration)
        {
            InvokeAction(contextManager, arguments, testTracer, out duration);
        }
    }
}