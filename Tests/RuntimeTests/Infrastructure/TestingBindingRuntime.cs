using System;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    public class TestingBindingRuntime : IBindingType
    {
        public string Name { get; private set; }
        public string FullName { get; private set; }

        public Type Type { get { throw new NotImplementedException (); } }

        public TestingBindingRuntime(string name, string fullName)
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

