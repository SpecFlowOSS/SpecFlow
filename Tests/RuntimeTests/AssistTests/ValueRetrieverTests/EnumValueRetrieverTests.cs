using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class EnumValueRetrieverTests
    {
        [Test]
        public void Throws_an_exception_when_the_value_is_not_an_enum()
        {
            var retriever = new EnumValueRetriever();

            var exceptionThrown = false;
            try
            {
                retriever.GetValue("NotDefinied", typeof (Sex));
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message == "No enum with value NotDefinied found")
                    exceptionThrown = true;
            }
            exceptionThrown.ShouldBeTrue();
        }

        [Test]
        public void Should_return_the_value_when_it_matches_the_enum()
        {
            var retriever = new EnumValueRetriever();
            retriever.GetValue("Male", typeof (Sex)).ShouldEqual(Sex.Male);
        }

        [Test]
        public void Should_return_the_value_when_it_includes_extra_spaces()
        {
            var retriever = new EnumValueRetriever();
            retriever.GetValue("Unknown Sex", typeof (Sex)).ShouldEqual(Sex.UnknownSex);
        }

        [Test]
        public void Returns_the_value_regardless_of_proper_casing()
        {
            var retriever = new EnumValueRetriever();
            retriever.GetValue("feMale", typeof (Sex)).ShouldEqual(Sex.Female);
        }

        [Test]
        public void Returns_the_proper_value_when_spaces_and_casing_is_wrong()
        {
            var retriever = new EnumValueRetriever();
            retriever.GetValue("unknown sex", typeof (Sex)).ShouldEqual(Sex.UnknownSex);
        }

        [Test]
        public void Throws_an_exception_when_the_value_is_null()
        {
            var retriever = new EnumValueRetriever();

            var exceptionThrown = false;
            try
            {
                retriever.GetValue(null, typeof (Sex));
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message == "No enum with value {null} found")
                    exceptionThrown = true;
            }
            exceptionThrown.ShouldBeTrue();
        }

        [Test]
        public void Throws_an_exception_when_the_value_is_empty()
        {
            var retriever = new EnumValueRetriever();

            var exceptionThrown = false;
            try
            {
                retriever.GetValue(string.Empty, typeof (Sex));
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message == "No enum with value {empty} found")
                    exceptionThrown = true;
            }
            exceptionThrown.ShouldBeTrue();
        }

        [Test]
        public void Does_not_throw_an_exception_when_the_value_is_null_and_enum_type_is_nullable()
        {
            var retriever = new EnumValueRetriever();

            var exceptionThrown = false;
            try
            {
                retriever.GetValue(null, typeof (Sex?));
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }
            exceptionThrown.ShouldBeFalse();
        }

        [Test]
        public void Does_not_throw_an_exception_when_the_value_is_empty_and_enum_type_is_nullable()
        {
            var retriever = new EnumValueRetriever();

            var exceptionThrown = false;
            try
            {
                retriever.GetValue(string.Empty, typeof (Sex?));
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }
            exceptionThrown.ShouldBeFalse();
        }

        [Test]
        public void Should_return_null_when_the_value_is_empty_and_enum_type_is_nullable()
        {
            var retriever = new EnumValueRetriever();
            retriever.GetValue(string.Empty, typeof(Sex?)).ShouldBeNull();
        }

        [Test]
        public void Should_return_the_value_when_it_matches_the_nullable_enum()
        {
            var retriever = new EnumValueRetriever();
            retriever.GetValue("Male", typeof (Sex?)).ShouldEqual(Sex.Male);
        }

        [Test]
        public void Should_return_the_value_when_it_includes_extra_spaces_on_the_nullable_enum()
        {
            var retriever = new EnumValueRetriever();
            retriever.GetValue("Unknown Sex", typeof (Sex?)).ShouldEqual(Sex.UnknownSex);
        }

        [Test]
        public void Returns_the_value_regardless_of_proper_casing_on_a_nullable_enum()
        {
            var retriever = new EnumValueRetriever();
            retriever.GetValue("feMale", typeof (Sex?)).ShouldEqual(Sex.Female);
        }

        [Test]
        public void Returns_the_proper_value_when_spaces_and_casing_is_wrong_on_a_nullable_enum()
        {
            var retriever = new EnumValueRetriever();
            retriever.GetValue("unknown sex", typeof (Sex?)).ShouldEqual(Sex.UnknownSex);
        }
    }
}