using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Constraints = Rhino.Mocks.Constraints;
using Should;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;
using TestStatus = TechTalk.SpecFlow.Infrastructure.TestStatus;
using Rhino.Mocks.Interfaces;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [Binding]
    public class BindingsForTableConverterTests
    {
        [Given("sample step with Table to Person conversion")]
        public virtual void PersonArg(Person param)
        {

        }

        [Given("sample step with Table to Person array conversion")]
        public virtual void PersonArrayArg(Person[] param)
        {

        }

        [Given("sample step with Table to IEnumerable<User> conversion")]
        public virtual void UserEnumerationArg(IEnumerable<User> param)
        {

        }

        [Given("sample step with Table to UserPerson conversion")]
        public virtual void UserPersonArg(UserPerson param)
        {
            
        }
        
        [Given("sample step with Table to UserWithConstructorParameters conversion")]
        public virtual void UserWithConstructorParametersArg(UserWithConstructorParameters param)
        {
            
        }
        
        [StepArgumentTransformation(@"user (\w+)")]
        public virtual User Create(string name)
        {
            return new User { Name = name };
        }
        
        [StepArgumentTransformation(@"(\w+) (\w+)")]
        public virtual Person Create(string firstName, string lastName)
        {
            return new Person { FirstName = firstName, LastName = lastName };
        }
    }

    public class UserPerson
    {
        public User User { get; set; }
        public Person Person { get; set; }
        public Guid Guid { get; set; }
        public Style Style { get; set; }
    }

    public class UserWithConstructorParameters
    {
        public int Id { get; private set; }
        public string Name { get; set; }

        public UserWithConstructorParameters()
	    {

	    }

        public UserWithConstructorParameters(int id)
	    {
            Id = id;
	    }
    }

    [TestFixture]
    public class TableConverterTests : StepExecutionTestsBase
    {
        private T ConversionTest<T>(
            string given, Action<BindingsForTableConverterTests, T> stepDefinition,
            Table table, bool shouldSucceed)
        {
            T result = default(T);
            BindingsForTableConverterTests bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.Create(null)).IgnoreArguments().CallOriginalMethod(OriginalCallOptions.NoExpectation);
            bindingInstance.Expect(b => b.Create(null, null)).IgnoreArguments().CallOriginalMethod(OriginalCallOptions.NoExpectation);
            bindingInstance.Expect(b => stepDefinition(b, default(T)))
                .IgnoreArguments().Repeat.Times(shouldSucceed ? 1 : 0)
                .WhenCalled(mi => result = (T)mi.Arguments[0]);
            MockRepository.ReplayAll();

            testRunner.Given(given, null, table);

            Assert.AreEqual(shouldSucceed ? TestStatus.OK : TestStatus.TestError, GetLastTestStatus());
            MockRepository.VerifyAll();

            return result;
        }

        private Person PersonConversionTest(Table table, bool shouldSucceed)
        {
            return ConversionTest<Person>("sample step with Table to Person conversion", (b, a) => b.PersonArg(a), table, shouldSucceed);
        }

        private Person[] PersonArrayConversionTest(Table table, bool shouldSucceed)
        {
            return ConversionTest<Person[]>("sample step with Table to Person array conversion", (b, a) => b.PersonArrayArg(a), table, shouldSucceed);
        }

        private IEnumerable<User> UserEnumerationConversionTest(Table table, bool shouldSucceed)
        {
            return ConversionTest<IEnumerable<User>>("sample step with Table to IEnumerable<User> conversion", (b, a) => b.UserEnumerationArg(a), table, shouldSucceed);
        }

        private UserPerson UserPersonConversionTest(Table table, bool shouldSucceed)
        {
            return ConversionTest<UserPerson>("sample step with Table to UserPerson conversion", (b, a) => b.UserPersonArg(a), table, shouldSucceed);
        }

        private UserWithConstructorParameters UserWithConstructorParametersConversionTest(Table table, bool shouldSucceed)
        {
            return ConversionTest<UserWithConstructorParameters>("sample step with Table to UserWithConstructorParameters conversion", (b, a) => b.UserWithConstructorParametersArg(a), table, shouldSucceed);
        }

        [Test]
        public void Table_converters_will_return_an_instance_of_T()
        {
            var table = new Table("Field", "Value");
            var result = PersonConversionTest(table, true);
            result.ShouldNotBeNull();
        }

        [Test]
        public void Table_converters_will_set_values_with_a_vertical_table_when_there_is_one_row_and_one_column()
        {
            var table = new Table("FirstName");
            table.AddRow("Homer");

            var result = PersonConversionTest(table, true);
            result.ShouldNotBeNull();
            result.FirstName.ShouldEqual("Homer");
        }

        [Test]
        public void When_one_row_exists_with_two_headers_and_the_first_row_value_is_not_a_property_then_treat_as_horizontal_table()
        {
            var table = new Table("FirstName", "LastName");
            table.AddRow("Homer", "Simpson");

            var result = PersonConversionTest(table, true);
            result.ShouldNotBeNull();
            result.FirstName.ShouldEqual("Homer");
            result.LastName.ShouldEqual("Simpson");
        }

        [Test]
        public void When_one_row_exists_with_two_headers_and_the_first_row_value_is_a_property_then_treat_as_a_vertical_table()
        {
            var table = new Table("FirstName", "LastName");
            table.AddRow("FirstName", "Homer");

            var result = PersonConversionTest(table, true);
            result.ShouldNotBeNull();
            result.FirstName.ShouldEqual("Homer");
            result.LastName.ShouldBeNull();
        }

        [Test]
        public void When_one_row_exists_with_two_headers_and_the_first_row_value_is_a_property_without_perfect_name_match_then_treat_as_a_vertical_table()
        {
            var table = new Table("FirstName", "LastName");
            table.AddRow("First name", "Homer");

            var result = PersonConversionTest(table, true);
            result.ShouldNotBeNull();
            result.FirstName.ShouldEqual("Homer");
            result.LastName.ShouldBeNull();
        }

        [Test]
        public void When_one_row_exists_with_three_headers_then_treat_as_horizontal_table()
        {
            var table = new Table("FirstName", "MiddleInitial", "LastName");
            table.AddRow("Homer", "J", "Simpson");

            var result = PersonConversionTest(table, true);
            result.ShouldNotBeNull();
            result.FirstName.ShouldEqual("Homer");
            result.MiddleInitial.ShouldEqual('J');
            result.LastName.ShouldEqual("Simpson");
        }

        [Test]
        public void Table_converters_will_return_an_empty_array_when_converting_an_empty_horizontal_table()
        {
            var table = new Table("FirstName", "MiddleInitial", "LastName");

            var result = PersonArrayConversionTest(table, true);
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Test]
        public void Table_converters_will_return_an_array_with_one_item_for_each_row_of_an_horizontal_table()
        {
            var table = new Table("FirstName", "MiddleInitial", "LastName");
            table.AddRow("Homer", "J", "Simpson");
            table.AddRow("Mona", "J", "Simpson");

            var result = PersonArrayConversionTest(table, true);
            result.ShouldNotBeNull();
            result.Length.ShouldEqual(2);
            result[0].ShouldNotBeNull();
            result[0].FirstName.ShouldEqual("Homer");
            result[1].ShouldNotBeNull();
            result[1].FirstName.ShouldEqual("Mona");
        }

        [Test]
        public void Table_converters_will_successfully_convert_an_horizontal_table_to_an_IEnumerable()
        {
            var table = new Table("Name");
            table.AddRow("Homer");
            table.AddRow("Mona");

            var result = UserEnumerationConversionTest(table, true);
            result.ShouldNotBeNull();
            result.Count().ShouldEqual(2);
            Assert.That(result.All(x => x != null), Is.True);
        }

        [Test]
        public void Table_converters_will_not_convert_a_vertical_table_to_an_array()
        {
            var table = new Table("Field", "Value");
            table.AddRow("FirstName", "Homer");
            table.AddRow("LastName", "Simpson");

            PersonArrayConversionTest(table, false);
        }

        [Test]
        public void Table_converters_will_not_convert_an_horizontal_with_multiple_rows_table_to_a_single_object()
        {
            var table = new Table("FirstName", "MiddleInitial", "LastName");
            table.AddRow("Homer", "J", "Simpson");
            table.AddRow("Mona", "J", "Simpson");

            PersonConversionTest(table, false);
        }

        [Test]
        public void Table_converters_will_use_other_converters_to_convert_each_value_from_the_table()
        {
            var table = new Table("User", "Person", "Guid", "Style");
            table.AddRow("user Admin", "Homer Simpson", "B48D8AF4-405F-4286-B83E-774EA773CFA3", "very cool");

            var result = UserPersonConversionTest(table, true);
            result.ShouldNotBeNull();
            result.User.ShouldNotBeNull();
            result.User.Name.ShouldEqual("Admin");
            result.Person.ShouldNotBeNull();
            result.Person.FirstName.ShouldEqual("Homer");
            result.Person.LastName.ShouldEqual("Simpson");
            result.Guid.ShouldEqual(new Guid("B48D8AF4-405F-4286-B83E-774EA773CFA3"));
            result.Style.ShouldEqual(Style.VeryCool);
        }

        [Test]
        public void Table_converters_will_try_to_convert_the_string_value_of_a_single_header_horizontal_table_if_there_is_no_conversion_available_for_the_pivoted_vertical_tables()
        {
            var table = new Table("Person");
            table.AddRow("Homer Simpson");
            table.AddRow("Mona Simpson");

            var result = PersonArrayConversionTest(table, true);
            result.ShouldNotBeNull();
            result.Length.ShouldEqual(2);
            result[0].ShouldNotBeNull();
            result[0].FirstName.ShouldEqual("Homer");
            result[0].LastName.ShouldEqual("Simpson");
            result[1].ShouldNotBeNull();
            result[1].FirstName.ShouldEqual("Mona");
            result[1].LastName.ShouldEqual("Simpson");
        }

        [Test]
        public void Table_converters_will_use_the_constructor_that_includes_the_maximum_number_of_columns_with_no_matching_writable_property()
        {
            var table = new Table("Id", "Name");
            table.AddRow("444", "Homer");

            var result = UserWithConstructorParametersConversionTest(table, true);
            result.ShouldNotBeNull();
            result.Id.ShouldEqual(444);
            result.Name.ShouldEqual("Homer");
        }

        [Test]
        public void Table_converters_will_ignore_columns_with_not_matching_writable_property_or_constructor_parameter()
        {
            var table = new Table("Id", "Name", "Comment");
            table.AddRow("444", "Homer", "Some comment");

            var result = UserWithConstructorParametersConversionTest(table, true);
            result.ShouldNotBeNull();
            result.Id.ShouldEqual(444);
            result.Name.ShouldEqual("Homer");
        }
    }
}
