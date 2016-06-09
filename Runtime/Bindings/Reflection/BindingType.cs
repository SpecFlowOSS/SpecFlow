using System;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class BindingType : IBindingType
    {
        public string Name { get; private set; }
        public string FullName { get; private set; }

        public BindingType(string name, string fullName)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (fullName == null) throw new ArgumentNullException("fullName");

            Name = name;
            FullName = fullName;
        }

        public override string ToString()
        {
            return "[" + FullName + "]";
        }

        protected bool Equals(BindingType other)
        {
            return string.Equals(Name, other.Name) && string.Equals(FullName, other.FullName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BindingType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (FullName != null ? FullName.GetHashCode() : 0);
            }
        }
    }
}