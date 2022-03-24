using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class BindingMethod : IBindingMethod
    {
        public IBindingType Type { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<IBindingParameter> Parameters { get; private set; }
        public IBindingType ReturnType { get; private set; }

        public BindingMethod(IBindingType type, string name, IEnumerable<IBindingParameter> parameters, IBindingType returnType)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            ReturnType = returnType;
        }

        public override string ToString()
        {
            return this.GetShortDisplayText();
        }

        protected bool Equals(BindingMethod other)
        {
            return Equals(Type, other.Type) && string.Equals(Name, other.Name) && Equals(Parameters, other.Parameters) && Equals(ReturnType, other.ReturnType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BindingMethod) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Parameters != null ? Parameters.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ReturnType != null ? ReturnType.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}