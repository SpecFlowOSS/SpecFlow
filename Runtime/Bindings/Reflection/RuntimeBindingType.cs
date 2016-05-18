using System;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class RuntimeBindingType : IBindingType
    {
        public readonly Type Type;

        public string Name
        {
            get { return Type.Name; }
        }

        public string FullName
        {
            get { return Type.FullName; }
        }

        public RuntimeBindingType(Type type)
        {
            this.Type = type;
        }

        public override string ToString()
        {
            return "[" + Type + "]";
        }
    }
}