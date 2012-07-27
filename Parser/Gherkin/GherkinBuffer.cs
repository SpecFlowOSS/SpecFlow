using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Parser.Gherkin
{
    public class GherkinBuffer
    {
        private readonly string content;
        private readonly int lineOffset;

        private int[] lineStarts;
        private int[] lineLengths;

        private int BufferLineCount
        {
            get { return lineStarts.Length; }
        }

        public int LineCount
        {
            get { return BufferLineCount + lineOffset; }
        }

        public bool IsFullBuffer
        {
            get { return lineOffset == 0; }
        }

        public GherkinBufferSpan EntireContentSpan { get; private set; }
        public GherkinBufferPosition StartPosition
        {
            get { return EntireContentSpan.StartPosition; }
        }
        public GherkinBufferPosition EndPosition
        {
            get { return EntireContentSpan.EndPosition; }
        }

        public int LineOffset
        {
            get { return lineOffset; }
        }

        public GherkinBuffer(string content): this(content, 0)
        {}

        public GherkinBuffer(string content, int lineOffset)
        {
            this.content = content;
            this.lineOffset = lineOffset;

            PrepareBuffer();
        }

        private int GetLineLength(int line)
        {
            AssertLineInBuffer(line);
            return lineLengths[line - lineOffset];
        }

        private int GetBufferPositionFromLine(int line)
        {
            AssertLineInBuffer(line);
            return lineStarts[line - lineOffset];
        }

        private void AssertLineInBuffer(int line)
        {
            if (line < lineOffset || line > LineCount)
                throw new Exception("this line is not in the buffer");
        }

        static private readonly Regex newLineRe = new Regex(@"\r?\n");
        private void PrepareBuffer()
        {
            var newLineMatches = newLineRe.Matches(content);

            lineStarts = new int[newLineMatches.Count + 1];
            lineLengths = new int[lineStarts.Length];

            int lineIndex = 0;
            lineStarts[0] = 0;
            foreach (Match newLineMatch in newLineMatches)
            {
                lineLengths[lineIndex] = newLineMatch.Index - lineStarts[lineIndex];
                lineIndex++;
                lineStarts[lineIndex] = newLineMatch.Index + newLineMatch.Length;
            }
            lineLengths[lineIndex] = content.Length - lineStarts[lineIndex];

            var lastLine = LineCount - 1;
            EntireContentSpan = new GherkinBufferSpan(this, 0, 0, lastLine, GetLineLength(lastLine));
        }

        public GherkinBufferSpan GetLineSpan(int line)
        {
            return GetLineRangeSpan(line, line);
        }

        public GherkinBufferSpan GetLineRangeSpan(int startLine, int endLine)
        {
            return new GherkinBufferSpan(this, startLine, 0, endLine, GetLineLength(endLine));
        }

        public GherkinBufferPosition GetLineStartPosition(int line)
        {
            return new GherkinBufferPosition(this, line, 0);
        }

        public GherkinBufferPosition GetLineEndPosition(int line)
        {
            return new GherkinBufferPosition(this, line, GetLineLength(line));
        }

        public IEnumerable<GherkinBufferPosition> GetMatchesForLine(Regex regex, int line)
        {
            int lineStartPosition = GetBufferPositionFromLine(line);
            int length = GetLineLength(line);
            int startPosition = lineStartPosition;
            int endPosition = startPosition + length;
            do
            {
                var match = regex.Match(content, startPosition, length);
                if (!match.Success)
                    yield break;
                yield return new GherkinBufferPosition(this, line, match.Index - lineStartPosition);
                startPosition = match.Index + 1;
                length = endPosition - startPosition;
            } while (length > 0);
        }

        public GherkinBufferPosition GetMatchForLine(Regex regex, int line)
        {
            int startPosition = GetBufferPositionFromLine(line);
            int length = GetLineLength(line);
            var match = regex.Match(content, startPosition, length);
            if (!match.Success)
                return null;
            return new GherkinBufferPosition(this, line, match.Index - startPosition);
        }

        public GherkinBufferPosition IndexOfTextForLine(string text, int line)
        {
            return IndexOfTextForLine(text, line, 0);
        }

        public GherkinBufferPosition IndexOfTextForLine(string text, int line, int skipLineCharacters)
        {
            int lineStartPosition = GetBufferPositionFromLine(line);
            int startPosition = lineStartPosition + skipLineCharacters;
            int length = GetLineLength(line);
            var index = content.IndexOf(text, startPosition, length - skipLineCharacters, StringComparison.InvariantCulture);
            if (index < 0)
                return null;
            return new GherkinBufferPosition(this, line, index - lineStartPosition);
        }

        public string GetContent()
        {
            return content;
        }

        public string GetContentFrom(int line)
        {
            if (line == 0)
                return content;

            AssertLineInBuffer(line);
            int startPosition = GetBufferPositionFromLine(line);
            return content.Substring(startPosition);
        }
    }

    public class GherkinBufferSpan
    {
        public GherkinBuffer Buffer
        {
            get { return StartPosition.Buffer; }
        }
        public GherkinBufferPosition StartPosition { get; set; }
        public GherkinBufferPosition EndPosition { get; set; }

        public GherkinBufferSpan(GherkinBufferPosition startPosition, GherkinBufferPosition endPosition)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }

        public GherkinBufferSpan(GherkinBuffer buffer, int startLine, int startLinePosition, int endLine, int endLinePosition)
        {
            StartPosition = new GherkinBufferPosition(buffer, startLine, startLinePosition);
            EndPosition = new GherkinBufferPosition(buffer, endLine, endLinePosition);
        }
    }

    public class GherkinBufferPosition
    {
        public GherkinBuffer Buffer { get; private set; }
        /// <summary>
        /// Zero-based indexing!
        /// </summary>
        public int Line { get; private set; }
        /// <summary>
        /// Zero-based indexing!
        /// </summary>
        public int LinePosition { get; private set; }

        public GherkinBufferPosition(GherkinBuffer buffer, int line): this(buffer, line, 0) 
        {}

        public GherkinBufferPosition(GherkinBuffer buffer, int line, int linePosition)
        {
            Buffer = buffer;
            Line = line;
            LinePosition = linePosition;
        }

        public GherkinBufferPosition ShiftByCharacters(int characterShift)
        {
            return new GherkinBufferPosition(Buffer, Line, LinePosition + characterShift);
        }

        public static bool operator<(GherkinBufferPosition p1, GherkinBufferPosition p2)
        {
            return p1.Line < p2.Line ||
                   (p1.Line == p2.Line && p1.LinePosition < p2.LinePosition);
        }

        public static bool operator>(GherkinBufferPosition p1, GherkinBufferPosition p2)
        {
            return p1.Line > p2.Line ||
                   (p1.Line == p2.Line && p1.LinePosition > p2.LinePosition);
        }
    }
}