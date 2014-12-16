using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.StepSuggestions;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public static class GherkinFileScopeExtensions
    {
        public static string FormatBlockFullTitle(string keyword, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return keyword;
            return keyword + ": " + text;
        }

        public static string FullTitle(this IGherkinFileBlock block)
        {
            return FormatBlockFullTitle(block.Keyword, block.Title);
        }

        public static string FullTitle(this GherkinStep keywordLine)
        {
            return keywordLine.Keyword + keywordLine.Text;
        }

        public static string FullTitle(this IScenarioOutlineExampleSet exampleSet)
        {
            return FormatBlockFullTitle(exampleSet.Keyword, exampleSet.Text);
        }

        public static IEnumerable<IGherkinFileBlock> GetAllBlocks(this IGherkinFileScope gherkinFileScope)
        {
            return
                Enumerable.Empty<IGherkinFileBlock>()
                    .AppendIfNotNull(gherkinFileScope.HeaderBlock)
                    .AppendIfNotNull(gherkinFileScope.BackgroundBlock)
                    .Concat(gherkinFileScope.ScenarioBlocks)
                    .AppendIfNotNull(gherkinFileScope.InvalidFileEndingBlock);
        }

        public static int TotalErrorCount(this IGherkinFileScope gherkinFileScope)
        {
            return gherkinFileScope.GetAllBlocks().Sum(block => block.Errors.Count());
        }

        public static int GetStartLine(this IGherkinFileBlock gherkinFileBlock)
        {
            return gherkinFileBlock.KeywordLine + gherkinFileBlock.BlockRelativeStartLine;
        }

        public static int GetEndLine(this IGherkinFileBlock gherkinFileBlock)
        {
            return gherkinFileBlock.KeywordLine + gherkinFileBlock.BlockRelativeEndLine;
        }

        public static SnapshotSpan CreateSpan(this IEnumerable<IGherkinFileBlock> changedBlocks, ITextSnapshot textSnapshot)
        {
            VisualStudioTracer.Assert(changedBlocks.Any(), "there is no changed block");

            int minLineNumber = changedBlocks.First().GetStartLine();
            int maxLineNumber = changedBlocks.Last().GetEndLine();

            var minLine = textSnapshot.GetLineFromLineNumber(minLineNumber);
            var maxLine = minLineNumber == maxLineNumber ? minLine : textSnapshot.GetLineFromLineNumber(maxLineNumber);
            return new SnapshotSpan(minLine.Start, maxLine.EndIncludingLineBreak);
        }

        public static SnapshotSpan CreateChangeSpan(this GherkinFileScopeChange gherkinFileScopeChange)
        {
            var textSnapshot = gherkinFileScopeChange.GherkinFileScope.TextSnapshot;
            if (gherkinFileScopeChange.EntireScopeChanged)
                return new SnapshotSpan(textSnapshot, 0, textSnapshot.Length);

            return gherkinFileScopeChange.ChangedBlocks.CreateSpan(textSnapshot);
        }

        public static IList<ClassificationSpan> GetClassificationSpans(this IGherkinFileScope gherkinFileScope, SnapshotSpan snapshotSpan)
        {
            if (gherkinFileScope == null)
                return new ClassificationSpan[0];

            var result = new List<ClassificationSpan>();
            foreach (var gherkinFileBlock in gherkinFileScope.GetAllBlocks())
            {
                result.AddRange(gherkinFileBlock.ClassificationSpans); //TODO: optimize
            }
            return result;
        }

        public static IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(this IGherkinFileScope gherkinFileScope, NormalizedSnapshotSpanCollection spans)
        {
            if (gherkinFileScope == null)
                return new ITagSpan<IOutliningRegionTag>[0];

            var result = new List<ITagSpan<IOutliningRegionTag>>();
            foreach (var gherkinFileBlock in gherkinFileScope.GetAllBlocks())
            {
                result.AddRange(gherkinFileBlock.OutliningRegions); //TODO: optimize
            }
            return result;
        }

        public static GherkinStep GetStepAtPosition(this IGherkinFileScope gherkinFileScope, int lineNumber)
        {
            IStepBlock block;
            return GetStepAtPosition(gherkinFileScope, lineNumber, out block);
        }

        public static GherkinStep GetStepAtPosition(this IGherkinFileScope gherkinFileScope, int lineNumber, out IStepBlock block)
        {
            block = GetStepBlockFromStepPosition(gherkinFileScope, lineNumber);
            if (block == null)
                return null;

            var blockRelativeLine = lineNumber - block.KeywordLine;
            return block.Steps.FirstOrDefault(s => s.BlockRelativeLine == blockRelativeLine);
        }

        public static IStepBlock GetStepBlockFromStepPosition(this IGherkinFileScope gherkinFileScope, int lineNumber)
        {
            return gherkinFileScope.GetAllBlocks().LastOrDefault(si => si.KeywordLine < lineNumber) as IStepBlock;
        }
 
        public static IEnumerable<GherkinStep> GetAllSteps(this IGherkinFileScope gherkinFileScope)
        {
            return gherkinFileScope.GetAllBlocks().OfType<IStepBlock>().SelectMany(b => b.Steps);
        }

        public static string GetFileScopedLabel(this StepInstance stepInstance)
        {
            var position = stepInstance as ISourceFilePosition;

            var simpleLabel = stepInstance.GetLabel();

            if (position == null)
                return simpleLabel;

            return string.Format("{0} ({1}, line {2})", simpleLabel, position.SourceFile, position.FilePosition.Line);
        }

        public static string GetLabel(this StepInstance stepInstance)
        {
            string inFilePositionText = stepInstance.StepContext.ScenarioTitle == null
                                            ? "'Backround' section"
                                            : string.Format("scenario \"{0}\"", stepInstance.StepContext.ScenarioTitle);

            return string.Format("\"{0}{1}\" in {2}", stepInstance.Keyword, stepInstance.Text, inFilePositionText);
        }

        public static IEnumerable<GherkinStep> GetAllStepsWithFirstExampleSubstituted(this IGherkinFileScope gherkinFileScope)
        {
            return gherkinFileScope.GetAllBlocks().OfType<IStepBlock>().SelectMany(b => b is IScenarioOutlineBlock ? GetSubstitutedSteps((IScenarioOutlineBlock)b) : b.Steps);
        }

        private static IEnumerable<GherkinStep> GetSubstitutedSteps(IScenarioOutlineBlock scenarioOutlineBlock)
        {
            var firstNonEmptyExampleSet = scenarioOutlineBlock.ExampleSets.FirstOrDefault(es => es.ExamplesTable != null && es.ExamplesTable.RowCount > 0);
            if (firstNonEmptyExampleSet == null)
                return scenarioOutlineBlock.Steps;

            return scenarioOutlineBlock.Steps.Select(step => GetSubstitutedStep(step, firstNonEmptyExampleSet.ExamplesTable.Rows.First()));
        }

        public static GherkinStep GetSubstitutedStep(this GherkinStep step, IScenarioOutlineBlock scenarioOutlineBlock)
        {
            var firstNonEmptyExampleSet = scenarioOutlineBlock.ExampleSets.FirstOrDefault(es => es.ExamplesTable != null && es.ExamplesTable.RowCount > 0);
            if (firstNonEmptyExampleSet == null)
                return step;

            return GetSubstitutedStep(step, firstNonEmptyExampleSet.ExamplesTable.Rows.First());
        }

        static private readonly Regex paramRe = new Regex(@"\<(?<param>[^\>]+)\>");
        private static GherkinStep GetSubstitutedStep(GherkinStep step, IDictionary<string, string> exampleDictionary)
        {
            var replacedText = paramRe.Replace(step.Text,
                                               match =>
                                                   {
                                                       string value;
                                                       return exampleDictionary.TryGetValue(match.Groups["param"].Value, out value) ? value : match.Value;
                                                   });
            return new GherkinStep(step.StepDefinitionType, step.StepDefinitionKeyword, replacedText, step.StepContext, step.Keyword, step.BlockRelativeLine);
        }
    }
}