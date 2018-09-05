using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class ServiceComponentListTests
    {
        [Test]
        public void Should_be_empty_when_created()
        {
            var sut = new ServiceComponentList<ITestComponent>();

            sut.Should().BeEmpty();
        }

        private interface ITestComponent
        {
        }
    }
}