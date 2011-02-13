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
            Regex regex = new Regex("^" + regexString + "$", RegexOptions);
            Regex = regex;
        }

        private object[] GetStepTransformationArguments(string stepSnippet)
        {
            var match = Regex.Match(stepSnippet);
            var argumentStrings = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();
            return argumentStrings;
        }

        public object Transform(string value, ITestTracer testTracer)
        {
            var arguments = GetStepTransformationArguments(value);
            return InvokeAction(arguments, testTracer);
        }

        public object Transform(Table value, ITestTracer testTracer)
        {
            return InvokeAction(new object[] { value }, testTracer);
        }
    }
}