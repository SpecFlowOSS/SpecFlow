using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public enum GherkinTextBufferChangeType
    {
        EntireFile,
        SingleLine,
        MultiLine
    }

    public class GherkinTextBufferChange
    {
        public GherkinTextBufferChangeType Type { get; private set; }

        public int StartLine { get; private set; }
        public int EndLine { get; private set; }
        public int StartPosition { get; private set; }
        public int EndPosition { get; private set; }

        public int LineCountDelta { get; private set; }
        public int PositionDelta { get; private set; }

        public ITextSnapshot ResultTextSnapshot { get; private set; }

        public GherkinTextBufferChange(GherkinTextBufferChangeType type, int startLine, int endLine, int startPosition, int endPosition, int lineCountDelta, int positionDelta, ITextSnapshot resultTextSnapshot)
        {
            Type = type;
            StartLine = startLine;
            EndLine = endLine;
            StartPosition = startPosition;
            EndPosition = endPosition;
            LineCountDelta = lineCountDelta;
            PositionDelta = positionDelta;
            ResultTextSnapshot = resultTextSnapshot;
        }

        public static GherkinTextBufferChange Merge(GherkinTextBufferChange change1, GherkinTextBufferChange change2)
        {
            VisualStudioTracer.Assert(change1.ResultTextSnapshot.Version.VersionNumber <= change2.ResultTextSnapshot.Version.VersionNumber, "different snapshot version numbers for merging");
            if (change1.Type == GherkinTextBufferChangeType.EntireFile || change2.Type == GherkinTextBufferChangeType.EntireFile)
                return CreateEntireBufferChange(change2.ResultTextSnapshot);

            return new GherkinTextBufferChange(
                change1.Type == GherkinTextBufferChangeType.MultiLine || change2.Type == GherkinTextBufferChangeType.MultiLine ? GherkinTextBufferChangeType.MultiLine : GherkinTextBufferChangeType.SingleLine, 
                Math.Min(change1.StartLine, change2.StartLine),
                Math.Max(change1.EndLine, change2.EndLine),
                Math.Min(change1.StartPosition, change2.StartPosition),
                Math.Max(change1.EndPosition, change2.EndPosition),
                change1.LineCountDelta + change2.LineCountDelta,
                change1.PositionDelta + change2.PositionDelta,
                change2.ResultTextSnapshot);
        }

        public static GherkinTextBufferChange CreateEntireBufferChange(ITextSnapshot textSnapshot)
        {
            return new GherkinTextBufferChange(GherkinTextBufferChangeType.EntireFile,
                0, textSnapshot.LineCount - 1, 0, textSnapshot.Length, 0, 0, textSnapshot);
        }
    }
}