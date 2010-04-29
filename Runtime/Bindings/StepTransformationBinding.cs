using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepTransformationBinding : MethodBinding
    {
        public Regex Regex { get; private set; }

        public StepTransformationBinding(string regexString, MethodInfo methodInfo)
            : base(methodInfo)
        {
            Regex regex = new Regex("^" + regexString + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            Regex = regex;
        }

        public string[] GetStepTransformationArguments(string stepSnippet)
        {
            var match = Regex.Match(stepSnippet);
            var argumentStrings = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();
            return argumentStrings;
        }

        public object Transform(string value)
        {
            var arguments = GetStepTransformationArguments(value);
            return BindingAction.DynamicInvoke(arguments);
        }
    }
}