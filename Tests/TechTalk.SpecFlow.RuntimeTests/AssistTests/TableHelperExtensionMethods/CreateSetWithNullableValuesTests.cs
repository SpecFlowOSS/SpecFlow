using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    
    public class CreateSetWithNullableValuesTests
    {
        public CreateSetWithNullableValuesTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        private static Table CreatePersonTableHeaders()
        {
            return new Table("FirstName", "LastName", "BirthDate", "NumberOfIdeas", "Salary", "IsRational");
        }

        [Fact]
        public void Can_set_a_nullable_datetime()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "4/28/2009", "3", "", "");

            var people = table.CreateSet<NullablePerson>();

            people.First().BirthDate.Should().Be(new DateTime(2009, 4, 28));
        }

        [Fact]
        public void Sets_a_nullable_datetime_to_null_when_the_value_is_empty()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "", "", "");

            var people = table.CreateSet<NullablePerson>();

            people.First().BirthDate.Should().Be(null);
        }

        [Fact]
        public void Can_set_a_nullable_bool()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "3", "", "true");

            var people = table.CreateSet<NullablePerson>();

            people.First().IsRational.Value.Should().BeTrue();
        }

        [Fact]
        public void Sets_a_nullable_bool_to_null_when_the_value_is_empty()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "", "", "");

            var people = table.CreateSet<NullablePerson>();

            people.First().IsRational.Should().Be(null);
        }

        [Fact]
        public void Sets_a_nullable_double_to_null_when_the_value_is_empty()
        {
            var table = new Table("NullableDouble");
            table.AddRow("");

            var people = table.CreateSet<NullablePerson>();

            people.First().NullableDouble.Should().Be(null);
        }

        [Fact]
        public void Sets_a_nullable_guid_to_null_when_the_value_is_empty()
        {
            var table = new Table("NullableGuid");
            table.AddRow("");

            var people = table.CreateSet<NullablePerson>();

            people.First().NullableGuid.Should().Be(null);
        }

        [Fact]
        public void Sets_a_nullable_char_to_null_when_the_value_is_empty()
        {
            var table = new Table("NullableChar");
            table.AddRow("");

            var people = table.CreateSet<NullablePerson>();

            people.First().NullableChar.Should().Be(null);
        }

        [Fact]
        public void Can_set_a_nullable_int()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "3", "", "true");

            var people = table.CreateSet<NullablePerson>();

            people.First().NumberOfIdeas.Should().Be(3);
        }

        [Fact]
        public void Sets_a_nullable_int_to_null_when_the_value_is_empty()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "", "", "");

            var people = table.CreateSet<NullablePerson>();

            people.First().NumberOfIdeas.Should().Be(null);
        }

        [Fact]
        public void Can_set_a_nullable_uint()
        {
            var table = new Table("NullableUnsignedInt");
            table.AddRow("3");

            var people = table.CreateSet<NullablePerson>();

            people.First().NullableUnsignedInt.Should().Be((uint?)3);
        }

        [Fact]
        public void Sets_a_nullable_uint_to_null_when_the_value_is_empty()
        {
            var table = new Table("NullableUnsignedInt");
            table.AddRow("");

            var people = table.CreateSet<NullablePerson>();

            people.First().NullableUnsignedInt.Should().Be(null);
        }

        [Fact]
        public void Can_set_a_nullable_decimal()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "", 4.01M.ToString(), "");

            var people = table.CreateSet<NullablePerson>();

            people.First().Salary.Value.Should().Be(4.01M);
        }
        
        [Fact]
        public void Sets_a_nullable_decimal_to_null_when_the_value_is_empty()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "", "", "");

            var people = table.CreateSet<NullablePerson>();

            people.First().Salary.Should().Be(null);
        }

        [Fact]
        public void Sets_a_nullable_float_to_null_when_the_value_is_empty()
        {
            var table = new Table("NullableFloat");
            table.AddRow("");

            var people = table.CreateSet<NullablePerson>();

            people.First().NullableFloat.Should().Be(null);
        }
    }
}
