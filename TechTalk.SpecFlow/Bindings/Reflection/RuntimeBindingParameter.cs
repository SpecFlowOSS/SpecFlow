using System.Reflection;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class RuntimeBindingParameter : IBindingParameter
    {
        private readonly ParameterInfo parameterInfo;

        public IBindingType Type
        {
            get { return new RuntimeBindingType(parameterInfo.ParameterType); }
        }

        public string ParameterName
        {
            get { return parameterInfo.Name; }
        }

        public RuntimeBindingParameter(ParameterInfo parameterInfo)
        {
            this.parameterInfo = parameterInfo;
        }

        public override string ToString()
        {
            return $"{ParameterName}: {Type}";
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
            return (parameterInfo != null ? parameterInfo.GetHashCode() : 0);
        }
    }
}