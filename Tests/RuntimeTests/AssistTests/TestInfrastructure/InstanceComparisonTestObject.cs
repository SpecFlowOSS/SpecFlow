using System;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure
{
    public class InstanceComparisonTestObject
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public Guid GuidProperty { get; set; }
        public bool BoolProperty { get; set; }
        public int? NullableIntProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public float FloatProperty { get; set; }
    }
}