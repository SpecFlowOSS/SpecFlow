﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    [TestFixture]
    public class CreateSetWithNullableValuesTests
    {
        [SetUp]
        public void SetUp()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        private static Table CreatePersonTableHeaders()
        {
            return new Table("FirstName", "LastName", "BirthDate", "NumberOfIdeas", "Salary", "IsRational");
        }

        [Test]
        public void Can_set_a_nullable_datetime()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "4/28/2009", "3", "", "");

            var people = table.CreateSet<NullablePerson>();

            people.First().BirthDate.ShouldEqual(new DateTime(2009, 4, 28));
        }

        [Test]
        public void Sets_a_nullable_datetime_to_null_when_the_value_is_empty()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "", "", "");

            var people = table.CreateSet<NullablePerson>();

            people.First().BirthDate.ShouldBeNull();
        }

        [Test]
        public void Can_set_a_nullable_boolean()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "3", "", "true");

            var people = table.CreateSet<NullablePerson>();

            people.First().IsRational.Value.ShouldBeTrue();
        }

        [Test]
        public void Sets_a_nullable_boolean_to_null_when_the_value_is_empty()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "", "", "");

            var people = table.CreateSet<NullablePerson>();

            people.First().IsRational.ShouldBeNull();
        }

        [Test]
        public void Sets_a_nullable_double_to_null_when_the_value_is_empty()
        {
            var table = new Table("NullableDouble");
            table.AddRow("");

            var people = table.CreateSet<NullablePerson>();

            people.First().NullableDouble.ShouldBeNull();
        }

        [Test]
        public void Sets_a_nullable_guid_to_null_when_the_value_is_empty()
        {
            var table = new Table("NullableGuid");
            table.AddRow("");

            var people = table.CreateSet<NullablePerson>();

            people.First().NullableGuid.ShouldBeNull();
        }

        [Test]
        public void Sets_a_nullable_char_to_null_when_the_value_is_empty()
        {
            var table = new Table("NullableChar");
            table.AddRow("");

            var people = table.CreateSet<NullablePerson>();

            people.First().NullableChar.ShouldBeNull();
        }

        [Test]
        public void Can_set_a_nullable_int()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "3", "", "true");

            var people = table.CreateSet<NullablePerson>();

            people.First().NumberOfIdeas.ShouldEqual(3);
        }

        [Test]
        public void Sets_a_nullable_int_to_null_when_the_value_is_empty()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "", "", "");

            var people = table.CreateSet<NullablePerson>();

            people.First().NumberOfIdeas.ShouldBeNull();
        }

        [Test]
        public void Can_set_a_nullable_decimal()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "", 4.01M.ToString(), "");

            var people = table.CreateSet<NullablePerson>();

            people.First().Salary.Value.ShouldEqual(4.01M);
        }
        
        [Test]
        public void Sets_a_nullable_decimal_to_null_when_the_value_is_empty()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "", "", "");

            var people = table.CreateSet<NullablePerson>();

            people.First().Salary.ShouldBeNull();
        }

        [Test]
        public void Sets_a_nullable_single_to_null_when_the_value_is_empty()
        {
            var table = new Table("NullableSingle");
            table.AddRow("");

            var people = table.CreateSet<NullablePerson>();

            people.First().NullableSingle.ShouldBeNull();
        }
    }
}
