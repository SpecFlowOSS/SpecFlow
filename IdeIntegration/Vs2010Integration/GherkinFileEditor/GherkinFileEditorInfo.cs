using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    internal class ChangeInfo
    {
        private int startPosition = int.MaxValue;
        private int changeFirstLine = int.MaxValue;
        private int changeLastLine = 0;
        private int lineCountDelta = 0;
        private int positionDelta = 0;
        private readonly ITextSnapshot textSnapshot;

        public int StartPosition
        {
            get { return startPosition; }
        }

        public int ChangeFirstLine
        {
            get { return changeFirstLine; }
        }

        public int ChangeLastLine
        {
            get { return changeLastLine; }
        }

        public int LineCountDelta
        {
            get { return lineCountDelta; }
        }

        public int PositionDelta
        {
            get { return positionDelta; }
        }

        public ITextSnapshot TextSnapshot
        {
            get { return textSnapshot; }
        }

        public ChangeInfo(ITextBuffer buffer)
        {
            textSnapshot = buffer.CurrentSnapshot;
            startPosition = 0;
            changeFirstLine = 0;
            changeLastLine = textSnapshot.LineCount - 1;
            lineCountDelta = 0;
            positionDelta = 0;
        }

        public ChangeInfo(TextContentChangedEventArgs textContentChangedEventArgs)
        {
            this.textSnapshot = textContentChangedEventArgs.After;

            var beforeTextSnapshot = textContentChangedEventArgs.Before;
            foreach (var change in textContentChangedEventArgs.Changes)
            {
                startPosition = Math.Min(startPosition, change.OldPosition);
                changeFirstLine = Math.Min(changeFirstLine, beforeTextSnapshot.GetLineNumberFromPosition(change.OldPosition));
                changeLastLine = Math.Max(changeLastLine, beforeTextSnapshot.GetLineNumberFromPosition(change.OldEnd));
                lineCountDelta += change.LineCountDelta;
                positionDelta += change.Delta;
            }
        }

        public ChangeInfo Merge(TextContentChangedEventArgs textContentChangedEventArgs)
        {
            var result = new ChangeInfo(textContentChangedEventArgs);
            result.startPosition = Math.Min(result.startPosition, startPosition);
            result.changeFirstLine = Math.Min(result.changeFirstLine, changeFirstLine);
            result.changeLastLine = Math.Max(result.changeLastLine, changeLastLine);
            result.lineCountDelta += lineCountDelta;
            result.positionDelta += positionDelta;
            return result;
        }
    }

    internal class GherkinFileEditorInfo
    {
        public List<ClassificationSpan> HeaderClassificationSpans { get; private set; }
        public List<ScenarioEditorInfo> ScenarioEditorInfos { get; private set; }

        public GherkinFileEditorInfo()
        {
            HeaderClassificationSpans = new List<ClassificationSpan>();
            ScenarioEditorInfos = new List<ScenarioEditorInfo>();
        }
    }

    internal class ScenarioEditorInfo
    {
        public string Title { get; set; }
        public bool IsScenarioOutline { get; set; }
        public int StartLine { get; private set; }
        public int KeywordLine { get; set; }
        public ITagSpan<IOutliningRegionTag> ScenarioOutliningRegion { get; set; }
        public List<ClassificationSpan> ClassificationSpans { get; private set; }

        public bool IsComplete
        {
            get { return Title != null; }
        }

        public ScenarioEditorInfo(int startLine)
        {
            this.StartLine = startLine;
            this.ClassificationSpans = new List<ClassificationSpan>();
        }

        public ScenarioEditorInfo(ScenarioEditorInfo scenario, ITextSnapshot textSnapshot, int lineCountDelta, int positionDelta)
        {
            this.Title = scenario.Title;
            this.IsScenarioOutline = scenario.IsScenarioOutline;
            this.StartLine = scenario.StartLine + lineCountDelta;
            this.KeywordLine = scenario.KeywordLine + lineCountDelta;
            this.ScenarioOutliningRegion = scenario.ScenarioOutliningRegion.Shift(textSnapshot, positionDelta);
            this.ClassificationSpans = new List<ClassificationSpan>(
                scenario.ClassificationSpans.Select(cs => cs.Shift(textSnapshot, positionDelta)));
        }
    }

    internal static class TransformationExtensions
    {
        public static ScenarioEditorInfo Shift(this ScenarioEditorInfo oldScenario, ITextSnapshot newSnapshot, int lineCountDelta, int positionDelta)
        {
            return new ScenarioEditorInfo(oldScenario, newSnapshot, lineCountDelta, positionDelta);
        }

        public static ITagSpan<IOutliningRegionTag> Shift(this ITagSpan<IOutliningRegionTag> oldTagSpan, ITextSnapshot newSnapshot, int positionDelta)
        {
            if (oldTagSpan == null)
                return null;

            return new TagSpan<IOutliningRegionTag>(
                oldTagSpan.Span.Shift(newSnapshot, positionDelta),
                oldTagSpan.Tag);
        }

        public static ClassificationSpan Shift(this ClassificationSpan oldClassificationSpan, ITextSnapshot newSnapshot, int positionDelta)
        {
            return new ClassificationSpan(
                oldClassificationSpan.Span.Shift(newSnapshot, positionDelta),
                oldClassificationSpan.ClassificationType);
        }

        public static SnapshotSpan Shift(this SnapshotSpan oldSnapshotSpan, ITextSnapshot newSnapshot, int positionDelta)
        {
            return oldSnapshotSpan;
//            return new SnapshotSpan(
//                newSnapshot,
//                oldSnapshotSpan.Start + positionDelta,
//                oldSnapshotSpan.Length);
        }

        public static IEnumerable<TItem> TakeUntilItemExclusive<TItem>(this IEnumerable<TItem> list, TItem item)
        {
            return list.TakeWhile(it => !it.Equals(item));
        }

        public static IEnumerable<TItem> SkipFromItemInclusive<TItem>(this IEnumerable<TItem> list, TItem item)
        {
            return list.SkipWhile(it => !it.Equals(item));
        }
    }
}
