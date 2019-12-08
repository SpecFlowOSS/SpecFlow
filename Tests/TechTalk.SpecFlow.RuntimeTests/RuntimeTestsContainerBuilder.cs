using BoDi;
using Moq;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class RuntimeTestsContainerBuilder : ContainerBuilder
    {
        public RuntimeTestsContainerBuilder(IDefaultDependencyProvider defaultDependencyProvider = null)
            : base(defaultDependencyProvider)
        {
            
        }

        public Mock<IUnitTestRuntimeProvider> GetUnitTestRuntimeProviderMock()
        {
            return new Mock<IUnitTestRuntimeProvider>();
        }

        protected override void RegisterDefaults(ObjectContainer container)
        {
            base.RegisterDefaults(container);
            container.RegisterInstanceAs(GetUnitTestRuntimeProviderMock().Object, "nunit");
        }
    }
}
