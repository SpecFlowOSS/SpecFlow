using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Vs2010Integration.Bindings.Reflection;

namespace TechTalk.SpecFlow.Vs2010Integration.Bindings
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

        public bool Match(StepInstance stepInstance, bool useRegexMatching, bool useParamMatching = false, bool useScopeMatching = true)
        {
            if (useParamMatching)
                useRegexMatching = true;

            Match match = null;
            if (useRegexMatching && Regex != null && !(match = Regex.Match(stepInstance.Text)).Success)
                return false;

            if (BindingType != stepInstance.BindingType)
                return false;

            if (useScopeMatching && IsScoped && stepInstance.StepScope != null && !BindingScope.Match(stepInstance.StepScope))
                return false;

            if (useParamMatching)
            {
                Debug.Assert(match != null);
                var regexArgs = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();
                var arguments = regexArgs /*+ extraArgs*/;
                // check if the regex + extra arguments match to the binding method parameters
                if (arguments.Count() != Method.Parameters.Count())
                    return false;

                //TODO: proper param matching

                // Check if regex & extra arguments can be converted to the method parameters
//                if (bindingMatch.Arguments.Where(
//                    (arg, argIndex) => !CanConvertArg(arg, stepBinding.ParameterTypes[argIndex])).Any())
//                    return null;
            }

            return true;
        }
    }
}