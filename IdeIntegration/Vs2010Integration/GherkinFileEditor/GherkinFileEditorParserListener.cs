using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using TechTalk.SpecFlow.Parser.Gherkin;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    internal class GherkinFileEditorParserListener : IGherkinListener
    {
        private readonly GherkinFileEditorClassifications classifications;
        private readonly GherkinFileEditorInfo previousGherkinFileEditorInfo;
        private readonly GherkinFileEditorInfo gherkinFileEditorInfo;

        private GherkinBuffer gherkinBuffer;
        private readonly ITextSnapshot textSnapshot;

        private readonly int changeLastLine;
        private readonly int changeLineDelta;

        private bool isPartialParsing
        {
            get
            {
                return previousGherkinFileEditorInfo != null;
            }
        }

        public GherkinFileEditorParserListener(ITextSnapshot textSnapshot, GherkinFileEditorClassifications classifications, GherkinFileEditorInfo previousGherkinFileEditorInfo, int changeLastLine, int changeLineDelta)
        {
            this.textSnapshot = textSnapshot;
            this.changeLineDelta = changeLineDelta;
            this.changeLastLine = changeLastLine;
            this.classifications = classifications;
            this.previousGherkinFileEditorInfo = previousGherkinFileEditorInfo;
            gherkinFileEditorInfo = new GherkinFileEditorInfo();
        }

        public GherkinFileEditorInfo GetResult()
        {
            CloseScenario(textSnapshot.LineCount);

            return gherkinFileEditorInfo;
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

        private void ColorizeSpan(GherkinBufferSpan span, IClassificationType classificationType)
        {
            if (span == null)
                return;

            var startLine = textSnapshot.GetLineFromLineNumber(span.StartPosition.Line);
            var endLine = span.StartPosition.Line == span.EndPosition.Line ? startLine :
                textSnapshot.GetLineFromLineNumber(span.EndPosition.Line);
            var startIndex = startLine.Start + span.StartPosition.LinePosition;

            AddClassification(classificationType, 
                startIndex, 
                endLine.Start + span.EndPosition.LinePosition - startIndex);
        }

        private void ColorizeLinePart(string value, GherkinBufferSpan span, IClassificationType classificationType)
        {
            var textPosition = gherkinBuffer.IndexOfTextForLine(value, span.StartPosition.Line);
            if (textPosition == null)
                return;

            var textSpan = new GherkinBufferSpan(
                textPosition,
                textPosition.ShiftByCharacters(value.Length));
            ColorizeSpan(textSpan, classificationType);
        }

        private void RegisterKeyword(string keyword, GherkinBufferSpan headerSpan)
        {
            var keywordSpan = new GherkinBufferSpan(headerSpan.StartPosition,
                headerSpan.StartPosition.ShiftByCharacters(keyword.Length));
            ColorizeSpan(keywordSpan, classifications.Keyword);
        }

        private void ColorizeKeywordLine(string keyword, GherkinBufferSpan headerSpan, IClassificationType classificationType)
        {
            RegisterKeyword(keyword, headerSpan);
            //colorize the rest
            var textSpan = new GherkinBufferSpan(
                headerSpan.StartPosition.ShiftByCharacters(keyword.Length),
                headerSpan.EndPosition);
            ColorizeSpan(textSpan, classificationType);
        }

        private void AddOutline(int startLine, int endLine, string collapseText, string hooverText = null)
        {
            var lastScenario = gherkinFileEditorInfo.ScenarioEditorInfos.LastOrDefault();

            var tagSpans = lastScenario == null
                               ? gherkinFileEditorInfo.HeaderOutliningRegions
                               : lastScenario.OutliningRegions;

            int startPosition = textSnapshot.GetLineFromLineNumber(startLine).Start;
            int endPosition = textSnapshot.GetLineFromLineNumber(endLine).End;
            var span = new Span(startPosition, endPosition - startPosition);

            if (hooverText == null)
                hooverText = textSnapshot.GetText(span);

            tagSpans.Add(new TagSpan<IOutliningRegionTag>(
                             new SnapshotSpan(textSnapshot,
                                              span),
                             new OutliningRegionTag(false, false, collapseText, hooverText)));
        }

        private static readonly Regex commentRe = new Regex(@"^\s*#");
        private static readonly Regex whitespaceOnlyRe = new Regex(@"^\s*$");
        private void CloseScenario(int editorLine)
        {
            var lastScenario = gherkinFileEditorInfo.ScenarioEditorInfos.LastOrDefault();
            if (lastScenario != null && lastScenario.IsComplete && !lastScenario.IsClosed)
            {
                var regionStartLine = lastScenario.KeywordLine;
                var regionEndLine = editorLine - 1;
                // skip comments directly before next scenario start
                while (gherkinBuffer.GetMatchForLine(commentRe, regionEndLine) != null)
                    regionEndLine--;

                // skip empty lines directly before next scenario start (+ comments)
                while (gherkinBuffer.GetMatchForLine(whitespaceOnlyRe, regionEndLine) != null)
                    regionEndLine--;

                if (regionEndLine > regionStartLine)
                    AddOutline(
                        regionStartLine,
                        regionEndLine,
                        lastScenario.FullTitle);

                lastScenario.IsClosed = true;
            }
        }

        private void EnsureNewScenario(int editorLine)
        {
            CloseScenario(editorLine);

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

        public void Init(GherkinBuffer buffer, bool isPartialScan)
        {
            this.gherkinBuffer = buffer;
        }

        public void Comment(string commentText, GherkinBufferSpan commentSpan)
        {
            ColorizeSpan(commentSpan, classifications.Comment);
        }

        public void Feature(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            ColorizeKeywordLine(keyword, headerSpan, classifications.FeatureTitle);
            ColorizeSpan(descriptionSpan, classifications.Description);
        }

        public void Background(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            //TODO: outline
            RegisterKeyword(keyword, headerSpan);
            ColorizeSpan(descriptionSpan, classifications.Description);
        }

        public void Examples(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            //TODO: outline
            RegisterKeyword(keyword, headerSpan);
            ColorizeSpan(descriptionSpan, classifications.Description);
        }

        public void Scenario(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            ScenarioEditorInfo scenario = ProcessScenario(keyword, name, headerSpan, descriptionSpan);
            scenario.IsScenarioOutline = false;
        }

        public void ScenarioOutline(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            ScenarioEditorInfo scenario = ProcessScenario(keyword, name, headerSpan, descriptionSpan);
            scenario.IsScenarioOutline = true;
        }

        private ScenarioEditorInfo ProcessScenario(string keyword, string name, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            EnsureNewScenario(headerSpan.StartPosition.Line);

            ColorizeKeywordLine(keyword, headerSpan, classifications.ScenarioTitle);
            ColorizeSpan(descriptionSpan, classifications.Description);

            var scenario = gherkinFileEditorInfo.ScenarioEditorInfos.Last();
            scenario.KeywordLine = headerSpan.StartPosition.Line;
            scenario.Title = name;
            scenario.Keyword = keyword;
            return scenario;
        }

        public void FeatureTag(string name, GherkinBufferSpan tagSpan)
        {
            ColorizeSpan(tagSpan, classifications.Tag);
        }

        public void ScenarioTag(string name, GherkinBufferSpan tagSpan)
        {
            EnsureNewScenario(tagSpan.StartPosition.Line);
            ColorizeSpan(tagSpan, classifications.Tag);
        }

        public void ExamplesTag(string name, GherkinBufferSpan tagSpan)
        {
            ColorizeSpan(tagSpan, classifications.Tag);
        }

        private static readonly Regex placeholderRe = new Regex(@"\<.*?\>");
        public void Step(string keyword, StepKeyword stepKeyword, ScenarioBlock scenarioBlock, string text, GherkinBufferSpan stepSpan)
        {
            RegisterKeyword(keyword, stepSpan);

            var scenario = gherkinFileEditorInfo.ScenarioEditorInfos.LastOrDefault();
            if (scenario == null)
                return;
            if (scenario.IsScenarioOutline)
            {
                var matches = placeholderRe.Matches(text);
                foreach (Match match in matches)
                    ColorizeLinePart(match.Value, stepSpan, classifications.Placeholder);
            }
        }

        public void TableHeader(string[] cells, GherkinBufferSpan rowSpan, GherkinBufferSpan[] cellSpans)
        {
            foreach (var cellSpan in cellSpans)
            {
                ColorizeSpan(cellSpan, classifications.TableHeader);
            }
        }

        public void TableRow(string[] cells, GherkinBufferSpan rowSpan, GherkinBufferSpan[] cellSpans)
        {
            foreach (var cellSpan in cellSpans)
            {
                ColorizeSpan(cellSpan, classifications.TableCell);
            }
        }

        public void MultilineText(string text, GherkinBufferSpan textSpan)
        {
            ColorizeSpan(textSpan, classifications.MultilineText);
        }

        public void EOF(GherkinBufferPosition eofPosition)
        {
            //nop;
        }

        public void Error(string message, GherkinBufferPosition errorPosition, Exception exception)
        {
            //TODO
            var lastScenario = gherkinFileEditorInfo.ScenarioEditorInfos.LastOrDefault();
            if (lastScenario != null)
            {
                lastScenario.Errors++;
            }
            else
            {
                gherkinFileEditorInfo.HeaderErrors++;
            }
        }
    }
}