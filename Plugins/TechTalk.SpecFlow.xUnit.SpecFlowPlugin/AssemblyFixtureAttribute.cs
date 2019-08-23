// Contains code from https://github.com/xunit/samples.xunit
// originally published under Apache 2.0 license
// For more information see aforementioned repository
using System;

namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AssemblyFixtureAttribute : Attribute
    {
        public AssemblyFixtureAttribute(Type fixtureType)
        {
            FixtureType = fixtureType;
        }

        public Type FixtureType { get; }
    }
}
