using System;

namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin.Extensions.AssemblyFixture
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AssemblyFixtureAttribute : Attribute
    {
        public AssemblyFixtureAttribute(Type fixtureType)
        {
            FixtureType = fixtureType;
        }

        public Type FixtureType { get; private set; }
    }
}