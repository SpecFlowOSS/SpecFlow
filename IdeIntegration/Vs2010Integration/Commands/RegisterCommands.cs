using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoDi;
using EnvDTE;
using TechTalk.SpecFlow.Vs2010Integration.Commands;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    partial class DefaultDependencyProvider
    {
        static partial void RegisterCommands(IObjectContainer container)
        {
            var serviceProvider = container.Resolve<IServiceProvider>();

            container.RegisterInstanceAs<MenuCommandHandler>(new DelegateMenuCommandHandler(serviceProvider, container.Resolve<DTE>(),
                    (_1, _2) => System.Windows.MessageBox.Show("generate skeleton")), SpecFlowCmdSet.GenerateStepDefinitionSkeleton.ToString());
            container.RegisterTypeAs<RunScenariosCommand, MenuCommandHandler>(SpecFlowCmdSet.RunScenarios.ToString());
            container.RegisterTypeAs<DebugScenariosCommand, MenuCommandHandler>(SpecFlowCmdSet.DebugScenarios.ToString());
            container.RegisterTypeAs<ReGenerateAllCommand, MenuCommandHandler>(SpecFlowCmdSet.ReGenerateAll.ToString());
        }
    }
}
