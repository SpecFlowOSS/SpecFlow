using System;
using System.Collections.Generic;
using System.Linq;
using gherkin;
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
                changeLastLine = Math.Max(changeLastLine, textSnapshot.GetLineNumberFromPosition(change.NewEnd));
                //changeLastLine = Math.Max(changeLastLine, beforeTextSnapshot.GetLineNumberFromPosition(change.OldEnd));
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
        public I18n LanguageService { get; set; }

        public List<ClassificationSpan> HeaderClassificationSpans { get; private set; }
        public List<ITagSpan<IOutliningRegionTag>> HeaderOutliningRegions { get; set; }
        public int HeaderErrors { get; set; }
        public List<ScenarioEditorInfo> ScenarioEditorInfos { get; private set; }

        public int TotalErrorCount
        {
            get { return HeaderErrors + ScenarioEditorInfos.Sum(sei => sei.Errors); }
        }

        public GherkinFileEditorInfo()
        {
            HeaderClassificationSpans = new List<ClassificationSpan>();
            HeaderOutliningRegions = new List<ITagSpan<IOutliningRegionTag>>();
            ScenarioEditorInfos = new List<ScenarioEditorInfo>();
        }
    }

    internal class ScenarioEditorInfo
    {
        public string Title { get; set; }
        public string Keyword { get; set; }
        public bool IsScenarioOutline { get; set; }
        public int StartLine { get; private set; }
        public int KeywordLine { get; set; }
        public List<ITagSpan<IOutliningRegionTag>> OutliningRegions { get; set; }
        public List<ClassificationSpan> ClassificationSpans { get; private set; }
        public bool IsClosed { get; set; }
        public int Errors { get; set; }

        public bool IsComplete
        {
            get { return Title != null; }
        }

        public string FullTitle
        {
            get
            {
                return string.Format("{0}: {1}", Keyword, Title);
            }
        }

        public ScenarioEditorInfo(int startLine)
        {
            this.StartLine = startLine;
            this.ClassificationSpans = new List<ClassificationSpan>();
            this.OutliningRegions = new List<ITagSpan<IOutliningRegionTag>>();
            this.IsClosed = false;
        }

        public ScenarioEditorInfo(ScenarioEditorInfo scenario, ITextSnapshot textSnapshot, int lineCountDelta, int positionDelta)
        {
            this.Title = scenario.Title;
            this.Keyword = scenario.Keyword;
            this.IsScenarioOutline = scenario.IsScenarioOutline;
            this.IsClosed = scenario.IsClosed;
            this.StartLine = scenario.StartLine + lineCountDelta;
            this.KeywordLine = scenario.KeywordLine + lineCountDelta;
            this.ClassificationSpans = new List<ClassificationSpan>(
                scenario.ClassificationSpans.Select(cs => cs.Shift(textSnapshot, positionDelta)));
            this.OutliningRegions = new List<ITagSpan<IOutliningRegionTag>>(
                scenario.OutliningRegions);
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
