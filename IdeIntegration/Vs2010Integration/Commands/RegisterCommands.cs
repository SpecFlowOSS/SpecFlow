using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BoDi;
using EnvDTE;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Vs2010Integration.Commands;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.StepSuggestions;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

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

            container.RegisterInstanceAs<MenuCommandHandler>(new DelegateMenuCommandHandler(serviceProvider, container.Resolve<DTE>(),
                    (_1, selection) => GoToSteps(container.Resolve<IProjectScopeFactory>(), selection.DTE)), SpecFlowCmdSet.GoToSteps.ToString());
        }

        private static bool GoToSteps(IProjectScopeFactory projectScopeFactory, DTE dte)
        {
             if (dte.ActiveDocument == null || dte.ActiveDocument.ProjectItem == null)
                return false;

            var textSelection = (TextSelection)dte.ActiveDocument.Selection;
            if (textSelection == null)
                return false;
            var codeModel = dte.ActiveDocument.ProjectItem.FileCodeModel;
            if (codeModel == null)
                return false;
            CodeFunction currentMethod = null;
            try
            {
                currentMethod = codeModel.CodeElementFromPoint(textSelection.ActivePoint, vsCMElement.vsCMElementFunction) as CodeFunction;
            }
            catch(Exception)
            {
            }
            if (currentMethod == null)
                return false;

            var mFactory = new VsBindingReflectionFactory();
            var currentBindingMethod = mFactory.CreateBindingMethod(currentMethod);

            var stepSuggestionBindingCollector = new VsStepSuggestionBindingCollector();
            var stepDefinitionBinding = stepSuggestionBindingCollector.GetBindingsFromProjectItem(dte.ActiveDocument.ProjectItem).FirstOrDefault(sdb => sdb.Method.MethodEquals(currentBindingMethod));
            if (stepDefinitionBinding == null)
                return false;

            //System.Windows.MessageBox.Show("go to steps: " + stepDefinitionBinding.Method.ToString());
            //tracer.Trace(stepDefinitionBinding.ToString(), this);

            var projScope = projectScopeFactory.GetProjectScope(dte.ActiveDocument.ProjectItem.ContainingProject);

            var mathingInstances = projScope.StepSuggestionProvider.GetMatchingInstances(currentBindingMethod);
            var first = mathingInstances.FirstOrDefault(si => si is ISourceFilePosition);
            if (first == null)
                return false;

            var sfp = (ISourceFilePosition)first;

            var featureProjItem = VsxHelper.GetAllPhysicalFileProjectItem(dte.ActiveDocument.ProjectItem.ContainingProject).FirstOrDefault(
                pi => VsxHelper.GetProjectRelativePath(pi).Equals(sfp.SourceFile));

            if (featureProjItem == null)
                return false;

            if (!featureProjItem.IsOpen)
            {
                featureProjItem.Open();
            }
            GoToLine(featureProjItem, sfp.FilePosition.Line);

//            projScope.StepSuggestionProvider.GetConsideredStepDefinitions(stepDefinitionBinding.StepDefinitionType)
//                .Where(ssb => ssb.)

            return true;
        }

        private static void GoToLine(ProjectItem projectItem, int line)
        {
            TextDocument codeBehindTextDocument = (TextDocument) projectItem.Document.Object("TextDocument");

            EditPoint navigatePoint = codeBehindTextDocument.StartPoint.CreateEditPoint();
            navigatePoint.MoveToLineAndOffset(line, 1);
            navigatePoint.TryToShow();
            navigatePoint.Parent.Selection.MoveToPoint(navigatePoint);
        }
    }
}
