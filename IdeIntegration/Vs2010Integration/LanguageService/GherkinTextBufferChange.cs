using Microsoft.VisualStudio.Text;

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
    }
}