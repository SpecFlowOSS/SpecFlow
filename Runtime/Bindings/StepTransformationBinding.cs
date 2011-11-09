using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepTransformationBinding : MethodBinding
    {

#if SILVERLIGHT
        private static RegexOptions RegexOptions = RegexOptions.CultureInvariant;
#else
        private static RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
#endif

        public Regex Regex { get; private set; }

        public StepTransformationBinding(RuntimeConfiguration runtimeConfiguration, IErrorProvider errorProvider, string regexString, MethodInfo methodInfo)
            : base(runtimeConfiguration, errorProvider, methodInfo)
        {
            Regex = regexString == null ? null : new Regex("^" + regexString + "$", RegexOptions);
        }

        private object[] GetStepTransformationArgumentsFromRegex(string stepSnippet, IStepArgumentTypeConverter stepArgumentTypeConverter, CultureInfo cultureInfo)
        {
            var match = Regex.Match(stepSnippet);
            var argumentStrings = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value);
            return argumentStrings
                .Select((arg, argIndex) => stepArgumentTypeConverter.Convert(arg, ParameterTypes[argIndex], cultureInfo))
                .ToArray();
        }

        public object Transform(IContextManager contextManager, object value, ITestTracer testTracer, IStepArgumentTypeConverter stepArgumentTypeConverter, CultureInfo cultureInfo)
        {
            object[] arguments;
            if (Regex != null && value is string)
                arguments = GetStepTransformationArgumentsFromRegex((string)value, stepArgumentTypeConverter, cultureInfo);
            else
                arguments = new object[] {value};

            return InvokeAction(contextManager, arguments, testTracer);
        }
    }
}