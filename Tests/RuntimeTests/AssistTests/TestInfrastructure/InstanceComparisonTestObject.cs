using System;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure
{
    public class InstanceComparisonTestObject
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public Guid GuidProperty { get; set; }

        public bool BooleanProperty { get; set; }

        public int? NullableField { get; set; }

        public decimal DecimalProperty { get; set; }
    }
}