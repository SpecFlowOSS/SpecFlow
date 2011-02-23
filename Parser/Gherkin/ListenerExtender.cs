using System;
using System.Linq;
using System.Text.RegularExpressions;
using gherkin;
using java.util;

namespace TechTalk.SpecFlow.Parser.Gherkin
{
    internal class ListenerExtender : Listener
    {
        private readonly GherkinDialect gherkinDialect;
        private readonly IGherkinListener gherkinListener;

        private ScenarioBlock lastScenarioBlock = ScenarioBlock.Given;
        private bool inTable = false;
        private bool afterFeature = false;

        public int LineOffset { get; set; }
        public GherkinBuffer GherkinBuffer { get; private set; }

        public int lastProcessedEditorLine = -1;

        public int LastProcessedEditorLine
        {
            get { return lastProcessedEditorLine; }
        }

        public IGherkinListener GherkinListener
        {
            get { return gherkinListener; }
        }

        private bool IsIncremental
        {
            get { return !GherkinBuffer.IsFullBuffer; }
        }

        private void UpdateLastProcessedEditorLine(int editorLine)
        {
            lastProcessedEditorLine = Math.Max(editorLine, lastProcessedEditorLine);
        }

        private int GetEditorLine(int line)
        {
            return line - 1 + LineOffset;
        }

        private GherkinBufferPosition GetEOFPosition()
        {
            return GherkinBuffer.EndPosition;
        }

        private GherkinBufferSpan GetSingleLineSpanIgnoreWhitespace(int editorLine)
        {
            var startPosition = GetLineStartPositionIgnoreWhitespace(editorLine);
            var endPosition = GetLineEndPositionIgnoreWhitespace(editorLine);
            return new GherkinBufferSpan(startPosition, endPosition);
        }

        static private readonly Regex nonWhitespaceRe = new Regex(@"\S");
        private GherkinBufferPosition GetLineStartPositionIgnoreWhitespace(int editorLine)
        {
            var firstNonWSPosition = GherkinBuffer.GetMatchForLine(nonWhitespaceRe, editorLine);
            if (firstNonWSPosition != null)
                return firstNonWSPosition;
            return GherkinBuffer.GetLineStartPosition(editorLine);
        }

        static private readonly Regex lastNonWhitespaceRe = new Regex(@"\s*$");
        private GherkinBufferPosition GetLineEndPositionIgnoreWhitespace(int editorLine)
        {
            var lastNonWSPosition = GherkinBuffer.GetMatchForLine(lastNonWhitespaceRe, editorLine);
            if (lastNonWSPosition != null && lastNonWSPosition.LinePosition > 0)
                return lastNonWSPosition;
            return GherkinBuffer.GetLineEndPosition(editorLine);
        }

        private static readonly Regex whitespaceOnlyRe = new Regex(@"^\s*$");
        private GherkinBufferSpan GetDescriptionSpan(int titleEditorLine, string description)
        {
            if (string.IsNullOrEmpty(description) || whitespaceOnlyRe.Match(description).Success)
                return null;

            int descriptionStartLine = titleEditorLine + 1;
            while (GherkinBuffer.GetMatchForLine(whitespaceOnlyRe, descriptionStartLine) != null)
                descriptionStartLine++;

            int lineCount = GetLineCount(description);
            return GherkinBuffer.GetLineRangeSpan(descriptionStartLine, descriptionStartLine + lineCount - 1);
        }

        static private readonly Regex cellSeparatorRe = new Regex(@"\|");
        private GherkinBufferSpan[] GetCellSpans(int editorLine, string[] cells)
        {
            var cellSeparatorPositions = 
                GherkinBuffer.GetMatchesForLine(cellSeparatorRe, editorLine).ToArray();

            var result = new GherkinBufferSpan[cells.Length];
            for (int cellIndex = 0; cellIndex < cells.Length; cellIndex++)
            {
                if (cellSeparatorPositions.Length - 1 < cellIndex + 1)
                    break;

                result[cellIndex] = new GherkinBufferSpan(
                        cellSeparatorPositions[cellIndex].ShiftByCharacters(1),
                        cellSeparatorPositions[cellIndex + 1]);
            }

            return result;
        }

        private GherkinBufferSpan GetMultilineTextSpan(int editorLine, string text)
        {
            int lineCount = GetLineCount(text) + 2;
            return GherkinBuffer.GetLineRangeSpan(editorLine, editorLine + lineCount - 1);
        }

        static private readonly Regex newLineRe = new Regex(@"\r?\n");

        public ListenerExtender(GherkinDialect gherkinDialect, IGherkinListener gherkinListener, GherkinBuffer buffer)
        {
            this.gherkinDialect = gherkinDialect;
            this.gherkinListener = gherkinListener;
            this.GherkinBuffer = buffer;

            gherkinListener.Init(buffer, IsIncremental);
        }

        private int GetLineCount(string text)
        {
            return newLineRe.Matches(text).Count + 1;
        }

        private void ResetScenarioBlocks()
        {
            lastScenarioBlock = ScenarioBlock.Given;
        }

        private ScenarioBlock CalculateScenarioBlock(StepKeyword stepKeyword)
        {
            switch (stepKeyword)
            {
                case StepKeyword.Given:
                    lastScenarioBlock = ScenarioBlock.Given;
                    break;
                case StepKeyword.When:
                    lastScenarioBlock = ScenarioBlock.When;
                    break;
                case StepKeyword.Then:
                case StepKeyword.But:
                    lastScenarioBlock = ScenarioBlock.Then;
                    break;
                default:
                    // keep the existing one
                    break;
            }

            return lastScenarioBlock;
        }

        private GherkinBufferSpan ProcessSimpleLanguageElement(int line)
        {
            var editorLine = GetEditorLine(line);
            var bufferSpan = GetSingleLineSpanIgnoreWhitespace(editorLine);

            UpdateLastProcessedEditorLine(editorLine);
            return bufferSpan;
        }

        private GherkinBufferSpan ProcessComplexLanguageElement(int line, string description, out GherkinBufferSpan descriptionSpan)
        {
            var editorLine = GetEditorLine(line);
            var headerSpan = GetSingleLineSpanIgnoreWhitespace(editorLine);
            descriptionSpan = GetDescriptionSpan(editorLine, description);

            UpdateLastProcessedEditorLine(editorLine);
            if (descriptionSpan != null)
                UpdateLastProcessedEditorLine(descriptionSpan.EndPosition.Line);
            return headerSpan;
        }

        public void tag(string name, int line)
        {
            var bufferSpan = ProcessSimpleLanguageElement(line);

            if (IsIncremental || afterFeature)
                gherkinListener.ScenarioTag(name, bufferSpan);
            else
                gherkinListener.FeatureTag(name, bufferSpan);
        }

        public void comment(string comment, int line)
        {
            var bufferSpan = ProcessSimpleLanguageElement(line);

            gherkinListener.Comment(comment, bufferSpan);
        }

        public void location(string uri, int offset)
        {
            //TODO
        }

        public void feature(string keyword, string name, string description, int line)
        {
            if (afterFeature)
            {
                var editorLine = GetEditorLine(line);
                var errorPosition = GetLineStartPositionIgnoreWhitespace(editorLine);
                gherkinListener.Error("Duplicated feature title", errorPosition, null);
                return;
            }

            afterFeature = true;

            GherkinBufferSpan descriptionSpan;
            var headerSpan = ProcessComplexLanguageElement(line, description, out descriptionSpan);

            gherkinListener.Feature(keyword, name, description, headerSpan, descriptionSpan);
        }

        public void background(string keyword, string name, string description, int line)
        {
            ResetScenarioBlocks();

            GherkinBufferSpan descriptionSpan;
            var headerSpan = ProcessComplexLanguageElement(line, description, out descriptionSpan);

            gherkinListener.Background(keyword, name, description, headerSpan, descriptionSpan);
        }

        public void scenario(string keyword, string name, string description, int line)
        {
            ResetScenarioBlocks();

            GherkinBufferSpan descriptionSpan;
            var headerSpan = ProcessComplexLanguageElement(line, description, out descriptionSpan);

            gherkinListener.Scenario(keyword, name, description, headerSpan, descriptionSpan);
        }

        public void scenarioOutline(string keyword, string name, string description, int line)
        {
            ResetScenarioBlocks();

            GherkinBufferSpan descriptionSpan;
            var headerSpan = ProcessComplexLanguageElement(line, description, out descriptionSpan);

            gherkinListener.ScenarioOutline(keyword, name, description, headerSpan, descriptionSpan);
        }

        public void examples(string keyword, string name, string description, int line)
        {
            GherkinBufferSpan descriptionSpan;
            var headerSpan = ProcessComplexLanguageElement(line, description, out descriptionSpan);

            gherkinListener.Examples(keyword, name, description, headerSpan, descriptionSpan);
        }

        public void step(string keyword, string text, int line)
        {
            ResetStepArguments();

            var stepSpan = ProcessSimpleLanguageElement(line);

            StepKeyword stepKeyword = gherkinDialect.GetStepKeyword(keyword) ?? StepKeyword.And; // if we dont find it, we suppose an "and"
            ScenarioBlock scenarioBlock = CalculateScenarioBlock(stepKeyword);

            gherkinListener.Step(keyword, stepKeyword, scenarioBlock, text, stepSpan);
        }

        private void ResetStepArguments()
        {
            inTable = false;
        }

        public void row(List cellList, int line)
        {
            string[] cells = new string[cellList.size()];
            cellList.toArray(cells);

            var rowSpan = ProcessSimpleLanguageElement(line);
            GherkinBufferSpan[] cellSpans = GetCellSpans(rowSpan.StartPosition.Line, cells);
            if (!inTable)
            {
                inTable = true;
                gherkinListener.TableHeader(cells, rowSpan, cellSpans);
            }
            else
            {
                gherkinListener.TableRow(cells, rowSpan, cellSpans);
            }
        }

        public void pyString(string text, int line)
        {
            var editorLine = GetEditorLine(line);
            GherkinBufferSpan textSpan = GetMultilineTextSpan(editorLine, text);

            UpdateLastProcessedEditorLine(textSpan.EndPosition.Line);

            gherkinListener.MultilineText(text, textSpan);
        }

        public void eof()
        {
            var eofPosition = GetEOFPosition();

            UpdateLastProcessedEditorLine(eofPosition.Line);

            gherkinListener.EOF(eofPosition);
        }

        public void syntaxError(string state, string eventName, List legalEvents, int line)
        {
            //TODO
            var editorLine = GetEditorLine(line);
            var errorPosition = GetLineStartPositionIgnoreWhitespace(editorLine);

            string message = string.Format("Parser error. State: {0}, Event: {1}", state, eventName);

            UpdateLastProcessedEditorLine(editorLine);

            gherkinListener.Error(message, errorPosition, null);
        }
    }
}