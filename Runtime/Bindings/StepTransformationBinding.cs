using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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

        public StepTransformationBinding(string regexString, MethodInfo methodInfo)
            : base(methodInfo)
        {
            Regex = regexString == null ? null : new Regex("^" + regexString + "$", RegexOptions);
        }

        private object[] GetStepTransformationArgumentsFromRegex(string stepSnippet)
        {
            var match = Regex.Match(stepSnippet);
            var argumentStrings = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();
            return argumentStrings;
        }

        public object Transform(object value, ITestTracer testTracer)
        {
            object[] arguments;
            if (Regex != null && value is string)
                arguments = GetStepTransformationArgumentsFromRegex((string)value);
            else
                arguments = new object[] {value};

            return InvokeAction(arguments, testTracer);
        }
    }
}