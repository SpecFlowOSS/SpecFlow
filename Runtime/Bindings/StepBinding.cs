using System.Reflection;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepBinding : MethodBinding
    {
        public BindingType Type { get; private set; }
        public Regex Regex { get; private set; }

        public BindingScope BindingScope { get; private set; }
        public bool IsScoped { get { return BindingScope != null; } }

#if SILVERLIGHT
        private static RegexOptions RegexOptions = RegexOptions.CultureInvariant;
#else
        private static RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
#endif

        public StepBinding(BindingType type, string regexString, MethodInfo methodInfo, BindingScope bindingScope)
            : base(methodInfo)
        {
            Type = type;
            Regex regex = new Regex("^" + regexString + "$", RegexOptions);
            Regex = regex;
            this.BindingScope = bindingScope;
        }
    }
}