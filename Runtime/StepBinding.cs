using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Bindings
{
    //TODO: move to Bindigns folder
    public class StepBinding : MethodBinding
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