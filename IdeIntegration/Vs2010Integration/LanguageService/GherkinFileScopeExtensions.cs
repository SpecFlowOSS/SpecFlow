using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
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
            VisualStudioTracer.Assert(changedBlocks.Count() > 0, "there is no changed block");

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
            IStepBlock scenarioInfo = GetStepBlockFromStepPosition(gherkinFileScope, lineNumber);
            if (scenarioInfo == null)
                return null;

            var blockRelativeLine = lineNumber - scenarioInfo.KeywordLine;
            return scenarioInfo.Steps.FirstOrDefault(s => s.BlockRelativeLine == blockRelativeLine);
        }

        public static IStepBlock GetStepBlockFromStepPosition(this IGherkinFileScope gherkinFileScope, int lineNumber)
        {
            return gherkinFileScope.GetAllBlocks().LastOrDefault(si => si.KeywordLine < lineNumber) as IStepBlock;
        }
    }
}