using System;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class RuntimeBindingType : IBindingType
    {
        public string Name
        {
            get { return Type.Name; }
        }

        public string FullName
        {
            get { return Type.FullName; }
        }

        public Type Type { get; private set; }

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