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
    }
}