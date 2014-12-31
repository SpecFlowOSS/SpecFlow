using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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

	        var resultHandler = new MatchingMethodResultHandler(noMatchHandler, editorContext);

	        GetMatchingMethods(editorContext, bindingMatchService, projectCulture, resultHandler);
	        return resultHandler.Result;
        }

	    public static void GetMatchingMethods(GherkinEditorContext editorContext, IStepDefinitionMatchService bindingMatchService,
		    CultureInfo projectCulture, IMatchingMethodResultHandler resultHandler)
	    {
			var step = GetCurrentStep(editorContext);
		    if (step == null)
		    {
			    resultHandler.NoCurrentStep();
			    return;
		    }
			
			var bindingCulture = projectCulture ?? step.StepContext.Language;

		    if (!bindingMatchService.Ready)
		    {
			    resultHandler.BindingServiceNotReady();
			    return;
		    }

			StepDefinitionAmbiguityReason ambiguityReason;
			List<BindingMatch> candidatingMatches;
			var match = bindingMatchService.GetBestMatch(step, bindingCulture, out ambiguityReason, out candidatingMatches);

		    if (match.Success)
		    {
				//bindingMatchService.GetBestMatch()
			    match = GetNestedMatch(match, editorContext, step);
		    }

		    if (candidatingMatches.Any())
		    {
			    resultHandler.StepsFound(candidatingMatches, match);
				return;
		    }

		    resultHandler.NoMatchFound(bindingCulture, step);
	    }

	    private static BindingMatch GetNestedMatch(BindingMatch bindingMatch, GherkinEditorContext editorContext, GherkinStep step)
	    {
			// TODO: handle multiline steps
			var line = editorContext.TextView.TextSnapshot.GetLineFromLineNumber((int)editorContext.TextView.Caret.Top);
		    var regexMatch = bindingMatch.StepBinding.Regex.Match(step.Text);
		    var argumentMatches = regexMatch.Groups.Cast<Group>().Skip(1);

			var lineStart = line.Start;
			var column = editorContext.TextView.Caret.Position.BufferPosition - lineStart;

		    var parameterIndex = argumentMatches.Select((arg, index) => new {arg, index})
			    .SingleOrDefault(x => x.arg.Index + step.Keyword.Length <= column && x.arg.Index + x.arg.Length + step.Keyword.Length > column);

			Console.WriteLine("Parameter index={0}", parameterIndex == null ? -1 : parameterIndex.index);
		    if (parameterIndex == null)
			    return bindingMatch;
		    
			return bindingMatch;
	    }

	    public interface IMatchingMethodResultHandler
	    {
		    void NoCurrentStep();
		    void BindingServiceNotReady();
		    void StepsFound(List<BindingMatch> candidatingMatches, BindingMatch bindingMatch);
		    void NoMatchFound(CultureInfo bindingCulture, GherkinStep step);
	    }

	    private class MatchingMethodResultHandler : IMatchingMethodResultHandler
	    {
		    private readonly NoMatchHandler noMatchHandler;
		    private readonly GherkinEditorContext editorContext;

		    public MatchingMethodResultHandler(NoMatchHandler noMatchHandler, GherkinEditorContext editorContext)
		    {
			    Result = true;
			    this.noMatchHandler = noMatchHandler;
			    this.editorContext = editorContext;
		    }

		    public void NoCurrentStep()
		    {
			    Result = false;
		    }

		    public void BindingServiceNotReady()
		    {
				MessageBox.Show("Step bindings are still being analyzed. Please wait.", "Go to binding");
		    }

		    public void StepsFound(List<BindingMatch> candidatingMatches, BindingMatch bindingMatch)
		    {
				var match = bindingMatch;
				var binding = match.StepBinding;
				if (!match.Success)
				{
					WarnAboutMultipleMatches(candidatingMatches);
					binding = candidatingMatches.First().StepBinding;
				}

				var method = binding.Method;
				NavigateToMethod(editorContext, method);
		    }

		    public void NoMatchFound(CultureInfo bindingCulture, GherkinStep step)
		    {
			    noMatchHandler.HandleNoMatch(step, bindingCulture);
		    }

		    public bool Result { get; private set; }
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
