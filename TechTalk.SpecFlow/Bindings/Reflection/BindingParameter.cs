using System;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class BindingParameter : IBindingParameter
    {
        public IBindingType Type { get; private set; }
        public string ParameterName { get; private set; }

        public BindingParameter(IBindingType type, string parameterName)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        }

        public override string ToString()
        {
            return $"{ParameterName}: {Type}";
        }

        protected bool Equals(BindingParameter other)
        {
            return Equals(Type, other.Type) && string.Equals(ParameterName, other.ParameterName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BindingParameter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0)*397) ^ (ParameterName != null ? ParameterName.GetHashCode() : 0);
            }
        }
    }
}