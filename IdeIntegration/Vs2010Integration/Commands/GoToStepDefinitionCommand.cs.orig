using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Vs2010Integration.Bindings.Discovery;
using TechTalk.SpecFlow.Vs2010Integration.EditorCommands;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.IdeIntegration.Bindings;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public interface IGoToStepDefinitionCommand : IEditorCommand
    {
        bool CanGoToDefinition(GherkinEditorContext editorContext);
        bool GoToDefinition(GherkinEditorContext editorContext);
    }

    public class GoToStepDefinitionCommand : IGoToStepDefinitionCommand
    {
        private readonly IGherkinLanguageServiceFactory gherkinLanguageServiceFactory;
        private readonly IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider;

        public GoToStepDefinitionCommand(IServiceProvider serviceProvider, DTE dte, IGherkinLanguageServiceFactory gherkinLanguageServiceFactory, IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider) 
        {
            this.gherkinLanguageServiceFactory = gherkinLanguageServiceFactory;
            this.stepDefinitionSkeletonProvider = stepDefinitionSkeletonProvider;
        }

        public bool IsEnabled(Document activeDocument)
        {
            var editorContext = GherkinEditorContext.FromDocument(activeDocument, gherkinLanguageServiceFactory);
            if (editorContext == null) 
                return false;

            return CanGoToDefinition(editorContext);
        }

        public void Invoke(Document activeDocument)
        {
            var editorContext = GherkinEditorContext.FromDocument(activeDocument, gherkinLanguageServiceFactory);
            if (editorContext == null) 
                return;

            GoToDefinition(editorContext);
        }

        public bool CanGoToDefinition(GherkinEditorContext editorContext)
        {
            return GetBindingMatchService(editorContext.LanguageService) != null && GetCurrentStep(editorContext) != null;
        }

        public bool GoToDefinition(GherkinEditorContext editorContext)
        {
            var noMatchHandler = new NoMatchHandler(editorContext, stepDefinitionSkeletonProvider);

            var bindingMatchService = GetBindingMatchService(editorContext.LanguageService);
            if (bindingMatchService == null)
                return false;

            var projectCulture = editorContext.ProjectScope.SpecFlowProjectConfiguration.RuntimeConfiguration.BindingCulture;
            
            var matchResults = GetMatchingMethods(editorContext, bindingMatchService, projectCulture);

            var exitReason = matchResults.ExitReason;
            switch (exitReason)
            {
                case ExitReason.NoCurrentStep:
                    return false;

                case ExitReason.BindingServiceNotReady:
                    {
                        MessageBox.Show("Step bindings are still being analyzed. Please wait.", "Go to binding");
                        return true;
                    }

                case ExitReason.NoMatchFound:
                    {
                        noMatchHandler.HandleNoMatch(matchResults.Step, matchResults.BindingCulture);
                        return true;
                    }

                default:
                    var match = matchResults.BindingMatch;
                    var binding = match.StepBinding;
                    if (!match.Success)
                    {
                        var candidatingMatches = matchResults.CandidatingMatches;
                        WarnAboutMultipleMatches(candidatingMatches);
                        binding = candidatingMatches.First().StepBinding;
                    }

                    var method = binding.Method;
                    NavigateToMethod(editorContext, method);

                    return true;
            }
        }

        // TODO: refactor this method to accept a handler class for the various possible results instead of returning the "MatchResults" object
        // and get rid of MatchResults class because it's cohisiveness is very low
        public static MatchResults GetMatchingMethods(GherkinEditorContext editorContext, IStepDefinitionMatchService bindingMatchService, CultureInfo projectCulture)
        {
            var step = GetCurrentStep(editorContext);
            if (step == null)
                return new MatchResults(ExitReason.NoCurrentStep);

            var bindingCulture = projectCulture ?? step.StepContext.Language;

            if (!bindingMatchService.Ready)
                return new MatchResults(ExitReason.BindingServiceNotReady);
            
            StepDefinitionAmbiguityReason ambiguityReason;
            List<BindingMatch> candidatingMatches;
            var match = bindingMatchService.GetBestMatch(step, bindingCulture, out ambiguityReason, out candidatingMatches);
            return
                candidatingMatches.Any() ? new MatchResults(candidatingMatches, match) : new MatchResults(bindingCulture, step);
        }

        private static void WarnAboutMultipleMatches(IEnumerable<BindingMatch> candidatingMatches)
        {
            string bindingsText = string.Join(Environment.NewLine, candidatingMatches.Select(b => b.StepBinding.Method.GetShortDisplayText()));
            MessageBox.Show("Multiple matching bindings found. Navigating to the first match..."
                + Environment.NewLine + Environment.NewLine + bindingsText, "Go to binding");
        }

        private static void NavigateToMethod(GherkinEditorContext editorContext, IBindingMethod method)
        {
            var codeFunction = new VsBindingMethodLocator().FindCodeFunction(((VsProjectScope)editorContext.ProjectScope), method);

            if (codeFunction != null)
            {
                if (!codeFunction.ProjectItem.IsOpen)
                {
                    codeFunction.ProjectItem.Open();
                }
                var navigatePoint = codeFunction.GetStartPoint(vsCMPart.vsCMPartHeader);
                navigatePoint.TryToShow();
                navigatePoint.Parent.Selection.MoveToPoint(navigatePoint);
            }
        }

        private static IStepDefinitionMatchService GetBindingMatchService(GherkinLanguageService languageService)
        {
            var bindingMatchService = languageService.ProjectScope.BindingMatchService;
            if (bindingMatchService == null)
                return null;

            return bindingMatchService;
        }

        private static GherkinStep GetCurrentStep(GherkinEditorContext editorContext)
        {
            var fileScope = editorContext.LanguageService.GetFileScope(waitForLatest: true);
            if (fileScope == null)
                return null;

            SnapshotPoint caret = editorContext.TextView.Caret.Position.BufferPosition;
            IStepBlock block;
            var step = fileScope.GetStepAtPosition(caret.GetContainingLine().LineNumber, out block);

            if (step != null && block is IScenarioOutlineBlock)
                step = step.GetSubstitutedStep((IScenarioOutlineBlock)block);

            return step;
        }
    }
}
