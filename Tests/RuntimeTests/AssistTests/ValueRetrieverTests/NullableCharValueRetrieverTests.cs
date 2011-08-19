using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableCharValueRetrieverTests
    {
        [Test]
        public void Returns_the_value_from_the_char_value_retriever()
        {
            //var mock = new Mock<CharValueRetriever>();
            //mock.Setup(x => x.GetValue("the first test value")).Returns('a');
            //mock.Setup(x => x.GetValue("the second test value")).Returns('b');

            Func<string, char> func = value =>
            {
                if (value == "the first test value") return 'a';
                if (value == "the second test value") return 'b';
                return ' ';
            };
            
            var retriever = new NullableCharValueRetriever(func);
            retriever.GetValue("the first test value").ShouldEqual('a');
            retriever.GetValue("the second test value").ShouldEqual('b');
        }

        [Test]
        public void Returns_null_when_value_is_empty()
        {
            var retriever = new NullableCharValueRetriever(v => 'a');
            retriever.GetValue("").ShouldBeNull();
        }

        [Test]
        public void Returns_null_when_value_is_null()
        {
            var retriever = new NullableCharValueRetriever(v => 'a');
            retriever.GetValue(null).ShouldBeNull();
        }
    }
}