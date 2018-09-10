using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class RowExtensionMethodTests
    {
        [SetUp]
        public void SetUp()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        [Test]
        public void GetString_should_return_the_string_value_from_the_row()
        {
            var table = new Table("Name");
            table.AddRow("John Galt");
            table.Rows.First()
                .GetString("Name").Should().Be("John Galt");
        }

        [Test]
        public void GetString_should_return_null_if_the_value_is_not_defined()
        {
            var table = new Table("Name");
            table.AddRow("John Galt");
            table.Rows.First()
                .GetString("SomethingThatDoesNotExist").Should().Be(null);
        }

        [Test]
        public void GetInt_should_return_the_int_from_the_row()
        {
            var table = new Table("Count");
            table.AddRow("3");
            table.Rows.First()
                .GetInt32("Count").Should().Be(3);
        }

        [Test]
        public void GetInt_should_return_MinValue_when_the_value_is_not_defined()
        {
            var table = new Table("Count");
            table.AddRow("4");
            table.Rows.First()
                .GetInt32("SomethingThatDoesNotExist").Should().Be(int.MinValue);
        }

        [Test]
        public void GetInt_should_return_MinValue_when_the_value_is_empty()
        {
            var table = new Table("Count");
            table.AddRow("");
            table.Rows.First()
                .GetInt32("Count").Should().Be(int.MinValue);
        }

        [Test]
        public void GetDecimal_should_return_the_decimal_from_the_row()
        {
            var table = new Table("Amount");
            table.AddRow(4.01M.ToString());
            table.Rows.First()
                .GetDecimal("Amount").Should().Be(4.01M);
        }

        [Test]
        public void GetDecimal_should_return_MinValue_when_the_value_is_not_defined()
        {
            var table = new Table("Amount");
            table.AddRow("4.01");
            table.Rows.First()
                .GetDecimal("SomethingThatDoesNotExist").Should().Be(decimal.MinValue);
        }

        [Test]
        public void GetDecimal_should_return_MinValue_when_the_value_is_empty()
        {
            var table = new Table("Amount");
            table.AddRow("");
            table.Rows.First()
                .GetDecimal("Amount").Should().Be(decimal.MinValue);
        }

        [Test]
        public void GetDateTime_should_return_the_datetime_from_the_row()
        {
            var table = new Table("Birthdate");
            table.AddRow("4/28/2009 21:02:03");
            table.Rows.First()
                .GetDateTime("Birthdate").Should().Be(new DateTime(2009, 4, 28, 21, 2, 3));
        }

        [Test]
        public void GetDateTime_should_return_MinValue_when_the_value_is_not_defined()
        {
            var table = new Table("Birthdate");
            table.AddRow("4/28/2009 21:02:03");
            table.Rows.First()
                .GetDateTime("SomethingThatDoesNotExist").Should().Be(DateTime.MinValue);
        }

        [Test]
        public void GetDateTime_should_return_MinValue_when_the_value_is_empty()
        {
            var table = new Table("Birthdate");
            table.AddRow("");
            table.Rows.First()
                .GetDateTime("Birthdate").Should().Be(DateTime.MinValue);
        }

        [Test]
        public void GetBool_returns_true_when_the_value_is_true()
        {
            var table = new Table("IsNeat");
            table.AddRow("true");
            table.Rows.First()
                .GetBoolean("IsNeat").Should().BeTrue();
        }

        [Test]
        public void GetBool_returns_false_when_the_value_is_false()
        {
            var table = new Table("IsNeat");
            table.AddRow("false");
            table.Rows.First()
                .GetBoolean("IsNeat").Should().BeFalse();
        }

        [Test]
        public void GetBool_throws_an_exception_when_the_value_is_not_true_or_false()
        {
            var table = new Table("IsNeat");
            table.AddRow("is not true nor false");
            var exceptionThrown = false;
            try
            {
                table.Rows.First()
                    .GetBoolean("IsNeat");
            }
            catch (InvalidCastException exception)
            {
                if (exception.Message == "You must use 'true' or 'false' when setting bools for IsNeat")
                    exceptionThrown = true;
            }
            exceptionThrown.Should().BeTrue();
        }

        [Test]
        public void GetBool_returns_false_when_the_value_is_empty()
        {
            var table = new Table("IsNeat");
            table.AddRow("");
            table.Rows.First()
                .GetBoolean("IsNeat").Should().BeFalse();
        }

        [Test]
        public void GetBool_throws_an_exception_when_the_id_is_not_defined()
        {
            var table = new Table("IsNeat");
            table.AddRow("is not true nor false");
            var exceptionThrown = false;
            try
            {
                table.Rows.First()
                    .GetBoolean("SomethingThatDoesNotExist");
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message == "SomethingThatDoesNotExist could not be found in the row.")
                    exceptionThrown = true;
            }
            exceptionThrown.Should().BeTrue();
        }

        [Test]
        public void GetDouble_should_return_the_double_from_the_row()
        {
            var table = new Table("Amount");
            table.AddRow(4.01M.ToString());
            table.Rows.First()
                .GetDouble("Amount").Should().Be(4.01);
        }

        [Test]
        public void GetChar_should_return_the_character_from_the_row()
        {
            var table = new Table("Character");
            table.AddRow("M");
            table.Rows.First()
                .GetChar("Character").Should().Be('M');
        }

        [Test]
        public void GetDouble_should_return_MinValue_when_the_value_is_not_defined()
        {
            var table = new Table("Amount");
            table.AddRow("4.01");
            table.Rows.First()
                .GetDouble("SomethingThatDoesNotExist").Should().Be(double.MinValue);
        }

        [Test]
        public void GetDouble_should_return_MinValue_when_the_value_is_empty()
        {
            var table = new Table("Amount");
            table.AddRow("");
            table.Rows.First()
                .GetDouble("Amount").Should().Be(double.MinValue);
        }

        [Test]
        public void GetGuid_should_return_guid_version_of_string()
        {
            var table = new Table("Guid");
            table.AddRow("285B31CC-C5C2-4630-A1C5-EE7431717C3F");
            table.Rows.First()
                .GetGuid("Guid").Should().Be(new Guid("285B31CC-C5C2-4630-A1C5-EE7431717C3F"));
        }

        [Test]
        public void GetGuid_should_return_MinValue_when_the_value_is_not_defined()
        {
            var table = new Table("Guid");
            table.AddRow("285B31CC-C5C2-4630-A1C5-EE7431717C3F");
            table.Rows.First()
                .GetGuid("SomethingThatDoesNotExist").Should().Be(new Guid());
        }

        [Test]
        public void GetGuid_should_return_MinValue_when_the_value_is_empty()
        {
            var table = new Table("GetGuid");
            table.AddRow("");
            table.Rows.First()
                .GetGuid("GetGuid").Should().Be(new Guid());
        }

        [Test]
        public void GetSingle_should_return_the_single_from_the_row()
        {
            var table = new Table("Amount");
            table.AddRow(99.90F.ToString());
            table.Rows.First()
                .GetSingle("Amount").Should().Be(99.90F);
        }

        [Test]
        public void GetSingle_should_return_MinValue_when_the_value_is_empty()
        {
            var table = new Table("Amount");
            table.AddRow("");
            table.Rows.First()
                .GetSingle("Amount").Should().Be(Single.MinValue);
        }

        [Test]
        public void GetSingle_should_return_MinValue_When_the_value_is_not_defined()
        {
            var table = new Table("Amount");
            table.AddRow(11.11F.ToString());
            table.Rows.First()
                .GetSingle("SomethingThatDoesNotExist").Should().Be(Single.MinValue);
        }

        [Test]
        public void GetEnumValue_should_return_the_enum_field_form_the_row()
        {
            var table = new Table("Enum");
            table.AddRow("Male");
            var firstRow = table.Rows.First();

            firstRow.GetEnumValue<Sex>("Enum").Should().Be(Sex.Male);
        }

        [Test]
        public void GetEnumValue_should_throw_when_the_given_value_does_not_exist()
        {
            var table = new Table("Enum");
            table.AddRow("MemberDoesNotExist");
            var firstRow = table.Rows.First();

            Action act = () => firstRow.GetEnumValue<Sex>("Enum");

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void GetEnumValue_should_throw_when_the_given_value_does_not_match_case()
        {
            var table = new Table("Enum");
            table.AddRow("female");
            var firstRow = table.Rows.First();

            Action act = () => firstRow.GetEnumValue<Sex>("Enum");

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void GetSingle_should_throw_when_the_given_value_is_not_defined()
        {
            var table = new Table("Enum");
            table.AddRow("Female");
            var firstRow = table.Rows.First();

            Action act = () => firstRow.GetEnumValue<Sex>("SomethingThatDoesNotExist");

            act.ShouldThrow<IndexOutOfRangeException>();
        }
    }
}
