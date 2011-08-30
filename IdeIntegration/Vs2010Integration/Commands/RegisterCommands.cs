using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoDi;
using TechTalk.SpecFlow.Vs2010Integration.Commands;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    partial class DefaultDependencyProvider
    {
        static partial void RegisterCommands(IObjectContainer container)
        {
            var serviceProvider = container.Resolve<IServiceProvider>();

            container.RegisterInstanceAs<MenuCommandHandler>(new DelegateMenuCommandHandler(serviceProvider, PkgCmdIDList.cmdidGenerateStepDefinitionSkeleton,
                    (_1, _2) => System.Windows.MessageBox.Show("generate skeleton")), "cmdidGenerateStepDefinitionSkeleton");
            container.RegisterInstanceAs<MenuCommandHandler>(new DelegateMenuCommandHandler(serviceProvider, PkgCmdIDList.cmdidRunScenarios,
                    (_1, _2) => System.Windows.MessageBox.Show("run scenarios")), "cmdidRunScenarios");
        }
    }
}
