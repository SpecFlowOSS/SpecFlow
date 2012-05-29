using System;
using Moq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableGuidValueRetrieverTests
    {
        [Test]
        public void Returns_the_value_from_the_guid_retriever()
        {
            var mock = new Mock<IValueRetriever<Guid>>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns(new Guid());
            mock.Setup(x => x.GetValue("x")).Returns(new Guid("{A235BE7F-AFEE-43E8-BF01-F279A371662B}"));
            mock.Setup(x => x.GetValue("y")).Returns(new Guid("{08C91240-68EB-4B11-AFCC-95ED7D44EC67}"));

            var retriever = new NullableGuidValueRetriever(mock.Object);

            retriever.GetValue("x").ShouldEqual(new Guid("{A235BE7F-AFEE-43E8-BF01-F279A371662B}"));
            retriever.GetValue("y").ShouldEqual(new Guid("{08C91240-68EB-4B11-AFCC-95ED7D44EC67}"));
        }

        [Test]
        public void Returns_null_when_passed_null()
        {
            var mock = new Mock<IValueRetriever<Guid>>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns(new Guid("{A235BE7F-AFEE-43E8-BF01-F279A371662B}"));

            var retriever = new NullableGuidValueRetriever(mock.Object);
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_null_when_passed_empty()
        {
            var mock = new Mock<IValueRetriever<Guid>>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns(new Guid("{A235BE7F-AFEE-43E8-BF01-F279A371662B}"));

            var retriever = new NullableGuidValueRetriever(mock.Object);
            retriever.GetValue("").ShouldBeNull();
        }
    }
}