using System;

namespace TechTalk.SpecFlow.Generator
{
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