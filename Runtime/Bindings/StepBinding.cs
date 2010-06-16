using System.Reflection;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepBinding : MethodBinding
    {
        public BindingType Type { get; private set; }
        public Regex Regex { get; private set; }

#if SILVERLIGHT
        private static RegexOptions RegexOptions = RegexOptions.CultureInvariant;
#else
        private static RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
#endif

        public StepBinding(BindingType type, string regexString, MethodInfo methodInfo)
            : base(methodInfo)
        {
            Type = type;
            Regex regex = new Regex("^" + regexString + "$", RegexOptions);
            Regex = regex;
        }


    }
}