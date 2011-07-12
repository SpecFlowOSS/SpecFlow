using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepBindingNew
    {
        public IBindingMethod Method { get; private set; }
        public BindingType BindingType { get; private set; }
        public Regex Regex { get; private set; }

        public BindingScopeNew BindingScope { get; private set; }
        public bool IsScoped { get { return BindingScope != null; } }

        public StepBindingNew(IBindingMethod method, BindingType bindingType, Regex regex, BindingScopeNew bindingScope)
        {
            Method = method;
            BindingType = bindingType;
            Regex = regex;
            BindingScope = bindingScope;
        }
    }
}