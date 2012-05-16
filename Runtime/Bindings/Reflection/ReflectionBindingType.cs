using System;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class ReflectionBindingType : IBindingType
    {
        private readonly Type type;

        public string Name
        {
            get { return type.Name; }
        }

        public string FullName
        {
            get { return type.FullName; }
        }

        public ReflectionBindingType(Type type)
        {
            this.type = type;
        }
    }
}