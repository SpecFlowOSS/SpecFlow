using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using gherkin;
using java.util;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    internal class PartialListeningDoneException : Exception
    {
        public ScenarioEditorInfo FirstUnchangedScenario { get; private set; }

        public PartialListeningDoneException(ScenarioEditorInfo firstUnchangedScenario)
        {
            FirstUnchangedScenario = firstUnchangedScenario;
        }
    }

    internal class GherkinFileEditorParserListener : Listener
    {
        private int lineOffset;
        private readonly GherkinFileEditorClassifications classifications;
        private readonly GherkinFileEditorInfo previousGherkinFileEditorInfo;
        private readonly ITextSnapshot textSnapshot;
        private readonly GherkinFileEditorInfo gherkinFileEditorInfo;

        private bool isInTable = false;
        private bool beforeFeature = true;

        private readonly int changeLastLine;
        private readonly int changeLineDelta;

        public int LineOffset
        {
            get { return lineOffset; }
            set { lineOffset = value; }
        }

        private bool isPartialParsing
        {
            get
            {
                return previousGherkinFileEditorInfo != null;
            }
        }

        public GherkinFileEditorParserListener(ITextSnapshot textSnapshot, GherkinFileEditorClassifications classifications, GherkinFileEditorInfo previousGherkinFileEditorInfo, int lineOffset, int changeLastLine, int changeLineDelta)
        {
            this.textSnapshot = textSnapshot;
            this.changeLineDelta = changeLineDelta;
            this.changeLastLine = changeLastLine;
            this.lineOffset = lineOffset;
            this.classifications = classifications;
            this.previousGherkinFileEditorInfo = previousGherkinFileEditorInfo;
            gherkinFileEditorInfo = new GherkinFileEditorInfo();

            beforeFeature = !isPartialParsing;
        }

        public GherkinFileEditorInfo GetResult()
        {
            return gherkinFileEditorInfo;
        }

        private int GetEditorLine(int line)
        {
            return line - 1 + lineOffset;
        }

        private string GetLineText(int editorLine)
        {
            var snapshotLine = textSnapshot.GetLineFromLineNumber(editorLine);
            return snapshotLine.GetText();
        }

        private void AddClassification(IClassificationType classificationType, int startIndex, int length)
        {
            var lastScenario = gherkinFileEditorInfo.ScenarioEditorInfos.LastOrDefault();

            List<ClassificationSpan> classificationSpans = lastScenario == null
                                                               ? gherkinFileEditorInfo.HeaderClassificationSpans
                                                               : lastScenario.ClassificationSpans;
            classificationSpans.Add(
                new ClassificationSpan(
                    new SnapshotSpan(textSnapshot, new Span(startIndex, length)),
                    classificationType));
        }

        private void ColorizeLine(int editorLine, IClassificationType classificationType)
        {
            var snapshotLine = textSnapshot.GetLineFromLineNumber(editorLine);
            AddClassification(classificationType, snapshotLine.Start, snapshotLine.LengthIncludingLineBreak);
        }

        private void ColorizeLinePart(string lineTextPart, int editorLine, IClassificationType classificationType, int startIndex = 0)
        {
            var snapshotLine = textSnapshot.GetLineFromLineNumber(editorLine);

            int lineTextPartStartIndex = snapshotLine.GetText().IndexOf(lineTextPart, startIndex);
            if (lineTextPartStartIndex < 0)
                return;

            AddClassification(classificationType, snapshotLine.Start + lineTextPartStartIndex, lineTextPart.Length);
        }

        private void RegisterKeyword(string keyword, int editorLine)
        {
            ColorizeLinePart(keyword, editorLine, classifications.Keyword);
        }

        private void EnsureNewScenario(int editorLine)
        {
            if (isPartialParsing && editorLine > changeLastLine)
            {
                var firstUnchangedScenario =
                    previousGherkinFileEditorInfo.ScenarioEditorInfos.FirstOrDefault(
                        prevScenario => prevScenario.StartLine + changeLineDelta == editorLine);

                if (firstUnchangedScenario != null)
                    throw new PartialListeningDoneException(firstUnchangedScenario);
            }

            var lastScenario = gherkinFileEditorInfo.ScenarioEditorInfos.LastOrDefault();
            if (lastScenario == null || lastScenario.IsComplete)
            {
                gherkinFileEditorInfo.ScenarioEditorInfos.Add(
                    new ScenarioEditorInfo(editorLine));
            }
        }

        public void location(string str, int line)
        {

        }

        public void tag(string tagName, int line)
        {
            var editorLine = GetEditorLine(line);
            if (!beforeFeature)
            {
                EnsureNewScenario(editorLine);
            }
            ColorizeLinePart(tagName, editorLine, classifications.Tag);
        }

        public void feature(string keyword, string title, string description, int line)
        {
            var editorLine = GetEditorLine(line);
            beforeFeature = false;
            RegisterKeyword(keyword, editorLine);
        }

        public void background(string keyword, string str2, string str3, int line)
        {
            //TODO: outline
            RegisterKeyword(keyword, GetEditorLine(line));
        }

        public void scenario(string keyword, string name, string description, int line)
        {
            var editorLine = GetEditorLine(line);
            ScenarioEditorInfo scenario = ProcessScenario(editorLine, keyword, name);
            scenario.IsScenarioOutline = false;
        }

        public void scenarioOutline(string keyword, string name, string description, int line)
        {
            var editorLine = GetEditorLine(line);
            ScenarioEditorInfo scenario = ProcessScenario(editorLine, keyword, name);
            scenario.IsScenarioOutline = true;
        }

        private ScenarioEditorInfo ProcessScenario(int editorLine, string keyword, string name)
        {
            EnsureNewScenario(editorLine);

            RegisterKeyword(keyword, editorLine);
            ColorizeLinePart(name, editorLine, classifications.ScenarioTitle);

            var scenario = gherkinFileEditorInfo.ScenarioEditorInfos.Last();
            scenario.KeywordLine = editorLine;
            scenario.Title = name;
            return scenario;
        }

        public void examples(string keyword, string str2, string str3, int line)
        {
            //TODO: outline
            RegisterKeyword(keyword, GetEditorLine(line));
        }

        private static readonly Regex placeholderRe = new Regex(@"\<.*?\>");
        public void step(string keyword, string text, int line)
        {
            isInTable = false;

            var editorLine = GetEditorLine(line);
            var scenario = gherkinFileEditorInfo.ScenarioEditorInfos.LastOrDefault();
            if (scenario == null)
                return;

            RegisterKeyword(keyword, editorLine);
            if (scenario.IsScenarioOutline)
            {
                var matches = placeholderRe.Matches(text);
                foreach (Match match in matches)
                    ColorizeLinePart(match.Value, editorLine, classifications.Placeholder);
            }
        }

        public void row(List cellList, int line)
        {
            int editorLine = GetEditorLine(line);
            var lineText = GetLineText(editorLine);
            if (lineText == null)
                return;

            string[] cells = new string[cellList.size()];
            cellList.toArray(cells);

            IClassificationType classificationType = 
                isInTable ? classifications.TableCell : classifications.TableHeader;
            isInTable = true;

            int startIndex = lineText.IndexOf('|');
            if (startIndex < 0)
                return;
            foreach (var cell in cells)
            {
                ColorizeLinePart(cell.Trim(), editorLine, classificationType, startIndex);
                startIndex = lineText.IndexOf('|', startIndex + 1);
                if (startIndex < 0)
                    return;
            }
        }

        public void pyString(string text, int line)
        {
            int editorLine = GetEditorLine(line);
            int lineCount = text.Count(c => c.Equals('\n')) + 1 + 2;
            for (int currentLine = editorLine; currentLine < editorLine + lineCount; currentLine++)
            {
                ColorizeLine(currentLine, classifications.MultilineText);
            }
        }

        public void comment(string commentText, int line)
        {
            ColorizeLine(GetEditorLine(line), classifications.Comment);
        }

        public void eof()
        {

        }

        public void syntaxError(string str1, string str2, List l, int line)
        {

        }
    }
}
