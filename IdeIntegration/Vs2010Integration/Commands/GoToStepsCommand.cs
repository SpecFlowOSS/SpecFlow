using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Vs2010Integration.Bindings.Discovery;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.StepSuggestions;
using TechTalk.SpecFlow.Vs2010Integration.UI;
using TechTalk.SpecFlow.Vs2010Integration.Utils;
using TextSelection = EnvDTE.TextSelection;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public class GoToStepsCommand : MenuCommandHandler
    {
        private readonly IProjectScopeFactory projectScopeFactory;

        public GoToStepsCommand(IServiceProvider serviceProvider, DTE dte, IProjectScopeFactory projectScopeFactory) : base(serviceProvider, dte)
        {
            this.projectScopeFactory = projectScopeFactory;
        }

        protected override void BeforeQueryStatus(OleMenuCommand command, SelectedItems selection)
        {
            var activeDocument = dte.ActiveDocument;
            if (activeDocument == null || activeDocument.ProjectItem == null || activeDocument.ProjectItem.FileCodeModel == null)
            {
                command.Visible = false;
                return;
            }

            if (selection.Count != 1)
            {
                command.Enabled = false;
                return;
            }

            command.Enabled = IsStepDefinition(activeDocument);
        }

        internal bool IsStepDefinition(Document activeDocument)
        {
            var bindingMethod = GetSelectedBindingMethod(activeDocument);
            return bindingMethod != null && IsStepDefinition(bindingMethod, activeDocument);
        }

        protected override void Invoke(OleMenuCommand command, SelectedItems selection)
        {
            var activeDocument = dte.ActiveDocument;
            Invoke(activeDocument);
        }

        internal void Invoke(Document activeDocument)
        {
            var bindingMethod = GetSelectedBindingMethod(activeDocument);

            var projectScopes = GetProjectScopes(activeDocument).ToArray();
            if (projectScopes.Any(ps => !ps.StepSuggestionProvider.Populated))
            {
                MessageBox.Show("Step bindings are still being analyzed. Please wait.", "Go to steps");
                return;
            }
            var candidatingPositions =
                projectScopes.SelectMany(ps => GetMatchingSteps(bindingMethod, ps))
                    .Distinct(StepInstanceComparer.Instance)
                    .OrderBy(si => si, StepInstanceComparer.Instance)
                    .ToArray();

            if (candidatingPositions.Length == 0)
            {
                MessageBox.Show("No matching step found.", "Go to steps");
                return;
            }

            if (candidatingPositions.Length == 1)
            {
                JumpToStep(candidatingPositions[0]);
                return;
            }

            var menuBuilder = new ContextMenuBuilder();
            menuBuilder.Title = "Go to steps: Multiple matching steps found.";
            menuBuilder.AddRange(candidatingPositions.Select((si, index) =>
                                                             new ContextMenuBuilder.ContextCommandItem(
                                                                 GetStepInstanceGotoTitle(si),
                                                                 () => JumpToStep(si))));

            VsContextMenuManager.ShowContextMenu(menuBuilder.ToContextMenu(), dte);
        }

        private string GetStepInstanceGotoTitle(StepInstance stepInstance)
        {
            var position = ((ISourceFilePosition) stepInstance);

            string inFilePositionText = stepInstance.StepContext.ScenarioTitle == null ? "'Backround' section" :
                string.Format("scenario \"{0}\"", stepInstance.StepContext.ScenarioTitle);

            return string.Format("\"{0}{1}\" in {2} ({3}, line {4})", stepInstance.Keyword, stepInstance.Text, inFilePositionText, position.SourceFile, position.FilePosition.Line);
        }

        private void JumpToStep(StepInstance stepInstance)
        {
            var position = ((ISourceFilePosition) stepInstance);
            var featureProjItem = VsxHelper.GetAllPhysicalFileProjectItem(dte.ActiveDocument.ProjectItem.ContainingProject).FirstOrDefault(
                pi => VsxHelper.GetProjectRelativePath(pi).Equals(position.SourceFile));

            if (featureProjItem == null)
                return;

            if (!featureProjItem.IsOpen)
                featureProjItem.Open();
            GoToLine(featureProjItem, position.FilePosition.Line);

        }

        private static void GoToLine(ProjectItem projectItem, int line)
        {
            TextDocument codeBehindTextDocument = (TextDocument) projectItem.Document.Object("TextDocument");

            EditPoint navigatePoint = codeBehindTextDocument.StartPoint.CreateEditPoint();
            navigatePoint.MoveToLineAndOffset(line, 1);
            navigatePoint.TryToShow();
            navigatePoint.Parent.Selection.MoveToPoint(navigatePoint);
        }

        private class StepInstanceComparer : IEqualityComparer<StepInstance>, IComparer<StepInstance>
        {
            public static readonly StepInstanceComparer Instance = new StepInstanceComparer();

            public bool Equals(StepInstance si1, StepInstance si2)
            {
                var sp1 = (ISourceFilePosition) si1;
                var sp2 = (ISourceFilePosition) si2;
                return sp1.SourceFile.Equals(sp2.SourceFile, StringComparison.InvariantCultureIgnoreCase) && sp1.FilePosition.Line == sp2.FilePosition.Line;
            }

            public int GetHashCode(StepInstance obj)
            {
                return ((ISourceFilePosition) obj).SourceFile.GetHashCode();
            }

            public int Compare(StepInstance si1, StepInstance si2)
            {
                var sp1 = (ISourceFilePosition) si1;
                var sp2 = (ISourceFilePosition) si2;

                int result = StringComparer.InvariantCultureIgnoreCase.Compare(sp1.SourceFile, sp2.SourceFile);
                if (result == 0)
                    result = sp1.FilePosition.Line.CompareTo(sp2.FilePosition.Line);
                return result;
            }
        }

        private IEnumerable<IProjectScope> GetProjectScopes(Document activeDocument)
        {
            var projectScope = projectScopeFactory.GetProjectScope(activeDocument.ProjectItem.ContainingProject);
            //TODO: instead of getting the project scope of the step definition, we need to find the project scopes, where the step definition is included...
            if (projectScope != null && projectScope.StepSuggestionProvider != null)
                yield return projectScope;
        } 

        private IEnumerable<StepInstance> GetMatchingSteps(IBindingMethod bindingMethod, IProjectScope projectScope)
        {
            return projectScope.StepSuggestionProvider.GetMatchingInstances(bindingMethod)
                .Where(si => si is ISourceFilePosition);
        }

        private bool IsStepDefinition(IBindingMethod bindingMethod, Document activeDocument)
        {
            var vsBindingRegistryBuilder = new VsBindingRegistryBuilder();
            var stepDefinitionBinding = vsBindingRegistryBuilder.GetBindingsFromProjectItem(activeDocument.ProjectItem).FirstOrDefault(sdb => sdb.Method.MethodEquals(bindingMethod));
            return stepDefinitionBinding != null;
        }

        private IBindingMethod GetSelectedBindingMethod(Document activeDocument)
        {
            var codeFunction = GetSelectedFunction(activeDocument);
            if (codeFunction == null)
                return null;
                        
            var bindingReflectionFactory = new VsBindingReflectionFactory();
            return bindingReflectionFactory.CreateBindingMethod(codeFunction);
        }

        private CodeFunction GetSelectedFunction(Document activeDocument)
        {
            var textSelection = (TextSelection)activeDocument.Selection;
            if (textSelection == null)
                return null;
            var codeModel = activeDocument.ProjectItem.FileCodeModel;
            if (codeModel == null)
                return null;

            try
            {
                return codeModel.CodeElementFromPoint(textSelection.ActivePoint, vsCMElement.vsCMElementFunction) as CodeFunction;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
