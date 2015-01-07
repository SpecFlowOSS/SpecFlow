using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Bindings;
using System.Globalization;

namespace TechTalk.SpecFlow.IdeIntegration.Bindings
{
    //public class GoToDefinitionHelper
    //{
    //    // TODO: refactor this method to accept a handler class for the various possible results instead of returning the "MatchResults" object
    //    // and get rid of MatchResults class because it's cohisiveness is very low
    //    public static MatchResults GetMatchingMethods(GherkinEditorContext editorContext, IStepDefinitionMatchService bindingMatchService, CultureInfo projectCulture)
    //    {
    //        var step = GetCurrentStep(editorContext);
    //        if (step == null)
    //            return new MatchResults(ExitReason.NoCurrentStep);

    //        var bindingCulture = projectCulture ?? step.StepContext.Language;

    //        if (!bindingMatchService.Ready)
    //        {
    //            return new MatchResults(ExitReason.BindingServiceNotReady);
    //        }

    //        StepDefinitionAmbiguityReason ambiguityReason;
    //        List<BindingMatch> candidatingMatches;
    //        var match = bindingMatchService.GetBestMatch(step, bindingCulture, out ambiguityReason, out candidatingMatches);
    //        return
    //            candidatingMatches.Any() ? new MatchResults(candidatingMatches, match) : new MatchResults(bindingCulture, step);
    //    }

    //    public static StepInstance GetCurrentStep(GherkinEditorContext editorContext)
    //    {
    //        var fileScope = editorContext.LanguageService.GetFileScope(waitForLatest: true);
    //        if (fileScope == null)
    //            return null;

    //        SnapshotPoint caret = editorContext.TextView.Caret.Position.BufferPosition;
    //        IStepBlock block;
    //        var step = fileScope.GetStepAtPosition(caret.GetContainingLine().LineNumber, out block);

    //        if (step != null && block is IScenarioOutlineBlock)
    //            step = step.GetSubstitutedStep((IScenarioOutlineBlock)block);

    //        return step;
    //    }

    //}
}
