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

        [Test]
        public void Should_allow_the_addition_of_new_components()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            var component = new TestComponentImpl();
            sut.Register(component);

            sut.Should().Equal(component);
        }

        private interface ITestComponent
        {
        }

        private class TestComponentImpl : ITestComponent
        {
        }
    }
}