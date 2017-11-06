using System;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure
{
    public class SetComparisonTestObject
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }

        public DateTime DateTimeProperty { get; set; }

        public Guid GuidProperty { get; set; }

    }
}