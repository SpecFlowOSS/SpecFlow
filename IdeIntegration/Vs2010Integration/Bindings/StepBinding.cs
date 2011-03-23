using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepBinding
    {
        public IBindingMethod Method { get; private set; }
        public BindingType BindingType { get; private set; }
        public Regex Regex { get; private set; }

        public BindingScope BindingScope { get; private set; }
        public bool IsScoped { get { return BindingScope != null; } }

        public StepBinding(IBindingMethod method, BindingType bindingType, Regex regex, BindingScope bindingScope)
        {
            Method = method;
            BindingType = bindingType;
            Regex = regex;
            BindingScope = bindingScope;
        }
    }
}