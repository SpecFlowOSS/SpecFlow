using System;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IStepArgumentTransformationBinding : IBinding
    {
        
    }

    public class StepTransformationBinding : MethodBinding, IStepArgumentTransformationBinding
    {

#if SILVERLIGHT
        private static RegexOptions RegexOptions = RegexOptions.CultureInvariant;
#else
        private static RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
#endif

        public Regex Regex { get; private set; }

        public StepTransformationBinding(string regexString, IBindingMethod bindingMethod)
            : base(bindingMethod)
        {
            Regex = regexString == null ? null : new Regex("^" + regexString + "$", RegexOptions);
        }
    }
}