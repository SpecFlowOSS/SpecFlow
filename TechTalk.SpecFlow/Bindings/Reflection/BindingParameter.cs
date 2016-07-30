using System;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class BindingParameter : IBindingParameter
    {
        public IBindingType Type { get; private set; }
        public string ParameterName { get; private set; }

        public BindingParameter(IBindingType type, string parameterName)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (parameterName == null) throw new ArgumentNullException("parameterName");

            Type = type;
            ParameterName = parameterName;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", ParameterName, Type);
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