using System;
using System.Reflection;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class RuntimeBindingParameter : IBindingParameter
    {
        private readonly ParameterInfo parameterInfo;
        private RuntimeBindingType runtimeBindingType;

        public IBindingType Type
        {
            get { return runtimeBindingType ??= new RuntimeBindingType(parameterInfo.ParameterType); }
        }

        public string ParameterName
        {
            get { return parameterInfo.Name; }
        }

        public RuntimeBindingParameter(ParameterInfo parameterInfo)
        {
            this.parameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", ParameterName, Type);
        }

        protected bool Equals(RuntimeBindingParameter other)
        {
            return Equals(parameterInfo, other.parameterInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RuntimeBindingParameter) obj);
        }

        public override int GetHashCode()
        {
            return parameterInfo.GetHashCode();
        }
    }
}