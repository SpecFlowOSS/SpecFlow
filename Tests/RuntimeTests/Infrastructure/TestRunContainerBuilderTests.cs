using NUnit.Framework;
using TechTalk.SpecFlow.Configuration;
using Should;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    [TestFixture]
    public class TestRunContainerBuilderTests
    {
        [Test]
        public void Should_create_a_container()
        {
            var container = TestRunContainerBuilder.CreateContainer();
            container.ShouldNotBeNull();
        }

        [Test]
        public void Should_register_runtime_configuration_with_default_config()
        {
            var container = TestRunContainerBuilder.CreateContainer();
            container.Resolve<RuntimeConfiguration>().ShouldNotBeNull();
        }
    }
}
