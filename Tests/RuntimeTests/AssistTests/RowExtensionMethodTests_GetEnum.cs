using System;
using System.Linq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;
using RowExtensionMethods = TechTalk.SpecFlow.Assist.RowExtensionMethods;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class RowExtensionMethodTests_GetEnum
    {
        [Test]
        public void GetEnum_returns_enum_value_when_the_value_is_defined_in_the_type()
        {
            var table = new Table("Sex");
            table.AddRow("NotDefinied");

            var exceptionThrown = false;
            try
            {
                var e = RowExtensionMethods.GetEnum<Person>(table.Rows.First(), "Sex");
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message == "No enum with value NotDefinied found in type Person")
                    exceptionThrown = true;
            }
            exceptionThrown.ShouldBeTrue();
        }

        [Test]
        public void GetEnum_should_return_the_enum_value_from_the_row()
        {
            var table = new Table("Sex");
            table.AddRow("Male");

            ObjectAssertExtensions.ShouldEqual<Enum>(table.Rows.First()
                                                              .GetEnum<Person>("Sex"), Sex.Male);
        }

        [Test]
        public void GetEnum_should_return_the_enum_value_from_the_row_even_when_i_use_spaces()
        {
            var table = new Table("Sex");
            table.AddRow("Unknown Sex");

            ObjectAssertExtensions.ShouldEqual<Enum>(table.Rows.First()
                                                              .GetEnum<Person>("Sex"), Sex.UnknownSex);
        }

        [Test]
        public void GetEnum_should_return_the_enum_value_from_the_row_even_when_i_mess_up_the_casing()
        {
            var table = new Table("Sex");
            table.AddRow("feMale");

            ObjectAssertExtensions.ShouldEqual<Enum>(table.Rows.First()
                                                              .GetEnum<Person>("Sex"), Sex.Female);
        }

        [Test]
        public void GetEnum_should_return_the_enum_value_from_the_row_even_when_i_mess_up_the_casing_and_use_spaces()
        {
            var table = new Table("Sex");
            table.AddRow("unknown sex");

            ObjectAssertExtensions.ShouldEqual<Enum>(table.Rows.First()
                                                              .GetEnum<Person>("Sex"), Sex.UnknownSex);
        }

        [Test]
        public void GetEnum_throws_exception_when_the_value_is_not_defined_in_any_Enum_in_the_type()
        {
            var table = new Table("Sex");
            table.AddRow("NotDefinied");

            var exceptionThrown = false;
            try
            {
                var e = RowExtensionMethods.GetEnum<Person>(table.Rows.First(), "Sex");
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message == "No enum with value NotDefinied found in type Person")
                    exceptionThrown = true;
            }
            exceptionThrown.ShouldBeTrue();
        }

        [Test]
        public void GetEnum_throws_exception_when_the_value_is_not_defined_in_more_than_one_Enum_in_the_type()
        {
            var table = new Table("Sex");
            table.AddRow("Male");

            var exceptionThrown = false;
            try
            {
                var e = RowExtensionMethods.GetEnum<PersonWithStyle>(table.Rows.First(), "Sex");
            }
            catch (Exception exception)
            {
                if (exception.Message == "Found sevral enums with the value Male in type PersonWithStyle")
                    exceptionThrown = true;
            }
            exceptionThrown.ShouldBeTrue();
        }
    }
}