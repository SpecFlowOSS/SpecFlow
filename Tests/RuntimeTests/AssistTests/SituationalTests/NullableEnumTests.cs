using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.SituationalTests
{
    [TestFixture]
    public class NullableEnumTests
    {
        public enum TestEnum
        {
            Value1,
            Value2,
            Value3
        }

        public class TestEntity
        {
            public TestEnum? TestProperty { get; set; }
        }

        [Test]
        public void The_value_should_be_set_if_it_is_in_the_table()
        {
            var table = new Table("Field", "Value");
            table.AddRow("TestProperty", "Value2");

            var test = table.CreateInstance<TestEntity>();
            test.TestProperty.ShouldEqual(TestEnum.Value2);
        }
    }
}