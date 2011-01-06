using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public static class GherkinFileScopeExtensions
    {
        public static string FullTitle(this IGherkinFileBlock block)
        {
            return block.Keyword + block.Title;
        }

        public static IEnumerable<IGherkinFileBlock> GetAllBlocks(this IGherkinFileScope gherkinFileScope)
        {
            return
                gherkinFileScope.HeaderBlock
                    .AsSingleItemEnumerable<IGherkinFileBlock>()
                    .AppendIfNotNull(gherkinFileScope.BackgroundBlock)
                    .Concat(gherkinFileScope.ScenarioBlocks);
        }

        public static int GetStartLine(this IGherkinFileBlock gherkinFileBlock)
        {
            return gherkinFileBlock.KeywordLine + gherkinFileBlock.BlockRelativeStartLine;
        }

        public static SnapshotSpan CreateSpan(this IEnumerable<IGherkinFileBlock> changedBlocks, ITextSnapshot textSnapshot)
        {
            Debug.Assert(changedBlocks.Count() > 0);

            int minLineNumber = changedBlocks.First().GetStartLine();
            int maxLineNumber = changedBlocks.Last().GetStartLine();

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
    }
}