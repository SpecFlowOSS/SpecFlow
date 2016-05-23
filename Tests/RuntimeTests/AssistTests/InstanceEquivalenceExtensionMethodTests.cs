using System;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class InstanceEquivalenceExtensionMethodTests
    {
        [SetUp]
        public void Setup()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        [Test]
        public void throws_exception_with_descriptive_message_when_instance_is_null()
        {
            var table = new Table("Field", "Value");

            var exception = GetExceptionThrownByThisComparison(table, null);

            exception.Message.Should().Be("The item to compare was null.");
        }

        [Test]
        public void returns_true_if_instance_matches_horizontal_table()
        {
            var table = new Table("IntProperty", "BoolProperty", "StringProperty", "DateTimeProperty");
            table.AddRow("1", "true", "Test", "4/30/2016");

            var test = new InstanceComparisonTestObject()
            {
                IntProperty = 1, 
                BoolProperty = true,
                StringProperty = "Test",
                DateTimeProperty = new DateTime(2016, 4, 30)
            };

            var result = GetResultFromThisComparison(table, test);

            result.Should().Be(true);
        }

        [Test]
        public void returns_true_if_instance_matches_vertical_table()
        {
            var table = new Table("Field", "Value");
            table.AddRow("IntProperty", "1");
            table.AddRow("BoolProperty", "true");
            table.AddRow("StringProperty", "Test");
            table.AddRow("DateTimeProperty", "4/30/2016");

            var test = new InstanceComparisonTestObject
            {
                IntProperty = 1,
                BoolProperty = true,
                StringProperty = "Test",
                DateTimeProperty = new DateTime(2016, 4, 30)
            };

            var result = GetResultFromThisComparison(table, test);

            result.Should().Be(true);
        }

        [Test]
        public void returns_false_if_instance_values_do_not_match()
        {
            var table = new Table("IntProperty", "BoolProperty", "StringProperty", "DateTimeProperty");
            table.AddRow("1", "true", "Test", "4/30/2016");

            var test = new InstanceComparisonTestObject
            {
                IntProperty = 1,
                BoolProperty = true,
                StringProperty = "Test",
                DateTimeProperty = new DateTime(2016, 4, 1)
            };

            var result = GetResultFromThisComparison(table, test);

            result.Should().Be(false);
        }

        [Test]
        public void returns_false_if_instance_does_not_have_property()
        {
            var table = new Table("IntProperty", "BoolProperty", "StringProperty", "DateTimeProperty", "SomeMissingProperty");
            table.AddRow("1", "true", "Test", "4/30/2016", "MissingPropertyValue");

            var test = new InstanceComparisonTestObject
            {
                IntProperty = 1,
                BoolProperty = true,
                StringProperty = "Test",
                DateTimeProperty = new DateTime(2016, 4, 30)
            };

            var result = GetResultFromThisComparison(table, test);

            result.Should().Be(false);
        }

        private static bool GetResultFromThisComparison(Table table, InstanceComparisonTestObject test)
        {
            return table.IsEquivalentToInstance(test);
        }

        private static ComparisonException GetExceptionThrownByThisComparison(Table table, InstanceComparisonTestObject test)
        {
            try
            {
                table.IsEquivalentToInstance(test);
            }
            catch (ComparisonException ex)
            {
                return ex;
            }
            return null;
        }
    }
}
