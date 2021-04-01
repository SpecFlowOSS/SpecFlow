using System;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class RuntimeBindingType : IPolymorphicBindingType
    {
        public readonly Type Type;

        public string Name => Type.Name;

        public string FullName => Type.FullName;

        public string AssemblyName => Type.Assembly.GetName().Name;

        public RuntimeBindingType(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public bool IsAssignableTo(IBindingType baseType)
        {
            return Type.IsAssignableTo(baseType);
        }

        public override string ToString()
        {
            return "[" + Type + "]";
        }

        protected bool Equals(RuntimeBindingType other)
        {
            return Equals(Type, other.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RuntimeBindingType) obj);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public static readonly RuntimeBindingType Void = new RuntimeBindingType(typeof(void));
    }
}