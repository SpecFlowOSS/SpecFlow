using System;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class BindingType : IBindingType
    {
        public string Name { get; }
        public string FullName { get; }
        public string AssemblyName { get; }

        public BindingType(string name, string fullName, string assemblyName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            AssemblyName = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));
        }

        public override string ToString()
        {
            return $"[{AssemblyName}:{FullName}]";
        }

        protected bool Equals(BindingType other)
        {
            return string.Equals(Name, other.Name) && string.Equals(FullName, other.FullName) && string.Equals(AssemblyName, other.AssemblyName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BindingType)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (FullName != null ? FullName.GetHashCode() : 0) ^ (AssemblyName != null ? AssemblyName.GetHashCode() : 0);
            }
        }
    }
}