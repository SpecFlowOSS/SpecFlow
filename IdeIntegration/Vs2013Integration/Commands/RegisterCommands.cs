using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BoDi;
using TechTalk.SpecFlow.Vs2010Integration.Commands;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    partial class DefaultDependencyProvider
    {
        static partial void RegisterCommands(IObjectContainer container)
        {
            container.RegisterTypeAs<RunScenariosCommand, MenuCommandHandler>(SpecFlowCmdSet.RunScenarios.ToString());
            container.RegisterTypeAs<DebugScenariosCommand, MenuCommandHandler>(SpecFlowCmdSet.DebugScenarios.ToString());
            container.RegisterTypeAs<ReGenerateAllCommand, MenuCommandHandler>(SpecFlowCmdSet.ReGenerateAll.ToString());
            container.RegisterTypeAs<GoToStepDefinitionCommand, MenuCommandHandler>(SpecFlowCmdSet.GoToStepDefinition.ToString());
            container.RegisterTypeAs<GoToStepsCommand, MenuCommandHandler>(SpecFlowCmdSet.GoToSteps.ToString());
            container.RegisterTypeAs<ContextDependentNavigationCommand, MenuCommandHandler>(SpecFlowCmdSet.ContextDependentNavigation.ToString());
            container.RegisterTypeAs<GenerateStepDefinitionSkeletonCommand, MenuCommandHandler>(SpecFlowCmdSet.GenerateStepDefinitionSkeleton.ToString());
        }
    }
}
