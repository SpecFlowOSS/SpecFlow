using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoDi;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.Vs2010Integration.VS2012;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    public static class VsContainerBuilder
    {
        internal static DefaultDependencyProvider DefaultDependencyProvider = new Vs2012DependencyProvider();

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
}
