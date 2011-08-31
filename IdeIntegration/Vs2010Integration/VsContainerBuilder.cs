using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using BoDi;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Options;
using TechTalk.SpecFlow.Vs2010Integration.TestRunner;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    public static class VsContainerBuilder
    {
        internal static DefaultDependencyProvider DefaultDependencyProvider = new DefaultDependencyProvider();

        public static IObjectContainer CreateContainer(SpecFlowPackagePackage package)
        {
            var container = new ObjectContainer();

            container.RegisterInstanceAs(package);
            container.RegisterInstanceAs<IServiceProvider>(package);

            RegisterDefaults(container);

            BiDiContainerProvider.CurrentContainer = container; //TODO: avoid static field

            return container;
        }

        private static void RegisterDefaults(IObjectContainer container)
        {
            DefaultDependencyProvider.RegisterDefaults(container);
        }
    }

    internal partial class DefaultDependencyProvider
    {
        static partial void RegisterCommands(IObjectContainer container);

        public virtual void RegisterDefaults(IObjectContainer container)
        {
            var serviceProvider = container.Resolve<IServiceProvider>();
            RegisterVsDependencies(container, serviceProvider);

            container.RegisterTypeAs<IntegrationOptionsProvider, IIntegrationOptionsProvider>();
            container.RegisterInstanceAs<IIdeTracer>(VsxHelper.ResolveMefDependency<IVisualStudioTracer>(serviceProvider));

            container.RegisterTypeAs<TestRunnerEngine, ITestRunnerEngine>();
            container.RegisterTypeAs<TestRunnerGatewayProvider, ITestRunnerGatewayProvider>();
            container.RegisterTypeAs<MsTestRunnerGateway, ITestRunnerGateway>(TestRunnerTool.MsTest.ToString());

            RegisterCommands(container);
        }

        protected virtual void RegisterVsDependencies(IObjectContainer container, IServiceProvider serviceProvider)
        {
            var dte = serviceProvider.GetService(typeof(DTE)) as DTE;
            if (dte != null)
                container.RegisterInstanceAs(dte);
        }
    }

    public interface IBiDiContainerProvider
    {
        IObjectContainer ObjectContainer { get; }
    }

    [Export(typeof(IBiDiContainerProvider))]
    internal class BiDiContainerProvider : IBiDiContainerProvider
    {
        public static IObjectContainer CurrentContainer { get; internal set; }

        public IObjectContainer ObjectContainer
        {
            get { return CurrentContainer; }
        }
    }
}
