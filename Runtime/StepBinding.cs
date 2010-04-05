using System.Reflection;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow
{
    internal class StepBinding : MethodBinding
    {
        public BindingType Type { get; private set; }
        public Regex Regex { get; private set; }

        public StepBinding(BindingType type, string regexString, MethodInfo methodInfo)
            : base(methodInfo)
        {
            Type = type;
            Regex regex = new Regex("^" + regexString + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            Regex = regex;
        }
    }
}