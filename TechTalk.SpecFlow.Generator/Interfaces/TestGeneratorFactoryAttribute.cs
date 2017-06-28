using System;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    /// IMPORTANT
    /// This class is used for interop with the Visual Studio Extension
    /// DO NOT REMOVE OR RENAME FIELDS!
    /// This breaks binary serialization accross appdomains
    [AttributeUsage(AttributeTargets.Assembly)]
    public class TestGeneratorFactoryAttribute : Attribute
    {
        public string Type { get; private set; }

        public TestGeneratorFactoryAttribute(string type)
        {
            Type = type;
        }
    }
}