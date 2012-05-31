using System;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class VsBindingReflectionFactory
    {
        public IBindingType CreateBindingType(CodeTypeRef type)
        {
            if (type == null)
                return null;

            var fullName = type.AsFullName;

            if (string.IsNullOrWhiteSpace(fullName) || fullName.Equals(typeof(void).FullName))
                return null;

            int lastPeriodIndex = fullName.LastIndexOf('.');
            var name = lastPeriodIndex >= 0 ? fullName.Substring(lastPeriodIndex + 1) : fullName;

            return new BindingType(name, fullName);
        }

        public IBindingType CreateBindingType(CodeClass codeClass)
        {
            return new BindingType(codeClass.Name, codeClass.FullName);
        }

        public IBindingParameter CreateBindingParameter(CodeParameter codeParameter)
        {
            return new BindingParameter(
                    CreateBindingType(codeParameter.Type), 
                    codeParameter.Name);
        }

        public IBindingMethod CreateBindingMethod(CodeFunction codeFunction)
        {
            var parameters = codeFunction.Parameters.Cast<CodeParameter>().Select(CreateBindingParameter).ToArray();
            var type = CreateBindingType((CodeClass)codeFunction.Parent);
            var returnType = CreateBindingType(codeFunction.Type);
            return new BindingMethod(type, codeFunction.Name, parameters, returnType);
        }
    }
}