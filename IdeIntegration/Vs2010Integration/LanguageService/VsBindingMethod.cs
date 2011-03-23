using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class VsBindingType : IBindingType
    {
        public string Name { get; private set; }
        public string FullName { get; private set; }

        public bool IsAssignableTo(Type type)
        {
            if (type.FullName == FullName)
                return true;

            if (type.BaseType != null && IsAssignableTo(type.BaseType))
                return true;

            return type.GetInterfaces().Any(IsAssignableTo);
        }

        public VsBindingType(CodeTypeRef type)
        {
            FullName = type.AsFullName;
            int lastPeriodIndex = FullName.LastIndexOf('.');
            Name = lastPeriodIndex >= 0 ? FullName.Substring(lastPeriodIndex + 1) : FullName;
        }

        public VsBindingType(CodeClass codeClass)
        {
            FullName = codeClass.FullName;
            Name = codeClass.Name;
        }
    }

    internal class VsBindingParameter : IBindingParameter
    {
        public IBindingType Type { get; private set; }
        public string ParameterName { get; private set; }

        public VsBindingParameter(CodeParameter codeParameter)
        {
            ParameterName = codeParameter.Name;
            Type = new VsBindingType(codeParameter.Type);
        }
    }

    internal class VsBindingMethod : IBindingMethod
    {
        public string Name { get; private set; }
        public IBindingType Type { get; private set; }

        public IEnumerable<IBindingParameter> Parameters { get; private set; }

        public string ShortDisplayText
        {
            get { return string.Format("{2}.{0}({1})", Name, string.Join(", ", Parameters.Select(p => p.Type.Name)), Type.Name); }
        }

        public VsBindingMethod(CodeFunction codeFunction)
        {
            Name = codeFunction.Name;
            Parameters = codeFunction.Parameters.Cast<CodeParameter>().Select(p => new VsBindingParameter(p)).ToArray();
            Type = new VsBindingType((CodeClass)codeFunction.Parent);
        }
    }
}