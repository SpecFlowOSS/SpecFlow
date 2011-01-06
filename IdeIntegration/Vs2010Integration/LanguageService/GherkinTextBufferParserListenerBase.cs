using System;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class GherkinTextBufferParserListener : GherkinTextBufferParserListenerBase
    {
        public GherkinTextBufferParserListener(ITextSnapshot textSnapshot, GherkinFileEditorClassifications classifications) 
            : base(textSnapshot, classifications)
        {
        }
    }

    internal class PartialListeningDone2Exception : ScanningCancelledException
    {
        public IScenarioBlock FirstUnchangedScenario { get; private set; }

        public PartialListeningDone2Exception(IScenarioBlock firstUnchangedScenario)
        {
            FirstUnchangedScenario = firstUnchangedScenario;
        }
    }

    internal class GherkinTextBufferPartialParserListener : GherkinTextBufferParserListenerBase
    {
        private readonly IGherkinFileScope previousScope;
        private readonly int changeLastLine;
        private readonly int changeLineDelta;

        public GherkinTextBufferPartialParserListener(ITextSnapshot textSnapshot, GherkinFileEditorClassifications classifications, IGherkinFileScope previousScope, int changeLastLine, int changeLineDelta)
            : base(textSnapshot, classifications)
        {
            this.previousScope = previousScope;
            this.changeLastLine = changeLastLine;
            this.changeLineDelta = changeLineDelta;
        }

        protected override void OnScenarioBlockCreating(int editorLine)
        {
            base.OnScenarioBlockCreating(editorLine);

            if (editorLine > changeLastLine)
            {
                var firstUnchangedScenario = previousScope.ScenarioBlocks.FirstOrDefault(
                    prevScenario => prevScenario.GetStartLine() + changeLineDelta == editorLine);

                if (firstUnchangedScenario != null)
                    throw new PartialListeningDone2Exception(firstUnchangedScenario);
            }
        }
    }

    internal abstract class GherkinTextBufferParserListenerBase : IGherkinListener
    {
        private readonly GherkinFileEditorClassifications classifications;
        private readonly GherkinFileScope gherkinFileScope;

        private GherkinBuffer gherkinBuffer;
        private readonly ITextSnapshot textSnapshot;

        private GherkinFileBlockBuilder currentFileBlockBuilder;

        public GherkinFileBlockBuilder CurrentFileBlockBuilder
        {
            get
            {
                if (currentFileBlockBuilder == null)
                    throw new InvalidOperationException("There is no current file block");
                return currentFileBlockBuilder;
            }
            set { currentFileBlockBuilder = value; }
        }

        protected GherkinTextBufferParserListenerBase(ITextSnapshot textSnapshot, GherkinFileEditorClassifications classifications)
        {
            this.textSnapshot = textSnapshot;
            this.classifications = classifications;
            gherkinFileScope = new GherkinFileScope();
        }

        public IGherkinFileScope GetResult()
        {
            CloseBlock(textSnapshot.LineCount);

            return gherkinFileScope;
        }

        #region Colorizeing & Outlining
        private void AddClassification(IClassificationType classificationType, int startIndex, int length)
        {
            CurrentFileBlockBuilder.ClassificationSpans.Add(
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
            int startPosition = textSnapshot.GetLineFromLineNumber(startLine).Start;
            int endPosition = textSnapshot.GetLineFromLineNumber(endLine).End;
            var span = new Span(startPosition, endPosition - startPosition);

            if (hooverText == null)
                hooverText = textSnapshot.GetText(span);

            CurrentFileBlockBuilder.OutliningRegions.Add(new TagSpan<IOutliningRegionTag>(
                             new SnapshotSpan(textSnapshot, span),
                             new OutliningRegionTag(false, false, collapseText, hooverText)));
        }
        #endregion

        private static readonly Regex commentRe = new Regex(@"^\s*#");
        private static readonly Regex whitespaceOnlyRe = new Regex(@"^\s*$");
        private int CalculateRegionEndLine(int editorLine)
        {
            var regionEndLine = editorLine - 1;
            // skip comments directly before next scenario start
            while (gherkinBuffer.GetMatchForLine(commentRe, regionEndLine) != null)
                regionEndLine--;

            // skip empty lines directly before next scenario start (+ comments)
            while (gherkinBuffer.GetMatchForLine(whitespaceOnlyRe, regionEndLine) != null)
                regionEndLine--;
            return regionEndLine;
        }

        private void CloseBlock(int editorLine)
        {
            if (CurrentFileBlockBuilder.SupportsOutlining)
            {
                var regionStartLine = CurrentFileBlockBuilder.KeywordLine;
                int regionEndLine = CalculateRegionEndLine(editorLine);

                if (regionEndLine > regionStartLine)
                    AddOutline(
                        regionStartLine,
                        regionEndLine,
                        CurrentFileBlockBuilder.FullTitle);
            }

            BuildBlock(CurrentFileBlockBuilder);
            CurrentFileBlockBuilder = null;
        }

        protected virtual void BuildBlock(GherkinFileBlockBuilder blockBuilder)
        {
            blockBuilder.Build(gherkinFileScope);
        }

        private void CreateBlock(int editorLine)
        {
            CurrentFileBlockBuilder = new GherkinFileBlockBuilder(editorLine);
        }

        private void NewBlock(int editorLine)
        {
            CloseBlock(editorLine);
            CreateBlock(editorLine);
        }

        private void EnsureNewScenario(int editorLine)
        {
            if (CurrentFileBlockBuilder.IsComplete)
            {
                CloseBlock(editorLine);
                OnScenarioBlockCreating(editorLine);
                CreateBlock(editorLine);
            }
        }

        protected virtual void OnScenarioBlockCreating(int editorLine)
        {
            //nop;
        }

        public void Init(GherkinBuffer buffer, bool isPartialScan)
        {
            gherkinBuffer = buffer;

            InitializeFirstBlock(gherkinBuffer.GetLineStartPosition(gherkinBuffer.LineOffset));
            Debug.Assert(currentFileBlockBuilder != null);
        }

        protected virtual void InitializeFirstBlock(GherkinBufferPosition startPosition)
        {
            CreateBlock(startPosition.Line);
        }

        public void Comment(string commentText, GherkinBufferSpan commentSpan)
        {
            ColorizeSpan(commentSpan, classifications.Comment);
        }

        public void Feature(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            CurrentFileBlockBuilder.SetMainData(typeof(IHeaderBlock), headerSpan.StartPosition.Line, keyword, name);

            ColorizeKeywordLine(keyword, headerSpan, classifications.FeatureTitle);
            ColorizeSpan(descriptionSpan, classifications.Description);
        }

        public void Background(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            NewBlock(headerSpan.StartPosition.Line);
            CurrentFileBlockBuilder.SetMainData(typeof(IBackgroundBlock), headerSpan.StartPosition.Line, keyword, name);

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
            ProcessScenario(keyword, name, headerSpan, descriptionSpan, typeof(IScenarioBlock));
        }

        public void ScenarioOutline(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            ProcessScenario(keyword, name, headerSpan, descriptionSpan, typeof(IScenarioOutlineBlock));
        }

        private void ProcessScenario(string keyword, string name, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan, Type blockType)
        {
            EnsureNewScenario(headerSpan.StartPosition.Line);
            CurrentFileBlockBuilder.SetMainData(blockType, headerSpan.StartPosition.Line, keyword, name);

            ColorizeKeywordLine(keyword, headerSpan, classifications.ScenarioTitle);
            ColorizeSpan(descriptionSpan, classifications.Description);
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

        private static readonly Regex placeholderRe = new Regex(@"\<.*?\>");
        public void Step(string keyword, StepKeyword stepKeyword, Parser.Gherkin.ScenarioBlock scenarioBlock, string text, GherkinBufferSpan stepSpan)
        {
            RegisterKeyword(keyword, stepSpan);

            if (CurrentFileBlockBuilder.BlockType == typeof(IScenarioOutlineBlock))
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
            CurrentFileBlockBuilder.Errors.Add(new ErrorInfo());
        }
    }
}