using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableGuidValueRetrieverTests : AssistTestsBase
    {
        [Test]
        public void Returns_the_value_from_the_guid_retriever()
        {
            var retriever = new NullableGuidValueRetriever();
            retriever.GetValue("A235BE7F-AFEE-43E8-BF01-F279A371662B").ShouldEqual(new Guid("{A235BE7F-AFEE-43E8-BF01-F279A371662B}"));
            retriever.GetValue("08C91240-68EB-4B11-AFCC-95ED7D44EC67").ShouldEqual(new Guid("{08C91240-68EB-4B11-AFCC-95ED7D44EC67}"));
        }

        [Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableGuidValueRetriever();

            retriever.GetValue((string) null).ShouldBeNull();
        }

        [Test]
        public void Returns_null_when_passed_empty()
        {
            var retriever = new NullableGuidValueRetriever();

            retriever.GetValue("").ShouldBeNull();
        }
    }
}