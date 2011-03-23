using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class GherkinTextBufferParserListener : GherkinTextBufferParserListenerBase
    {
        public GherkinTextBufferParserListener(GherkinDialect gherkinDialect, ITextSnapshot textSnapshot, GherkinFileEditorClassifications classifications)
            : base(gherkinDialect, textSnapshot, classifications)
        {
        }
    }

    internal abstract class GherkinTextBufferParserListenerBase : IGherkinListener
    {
        private readonly GherkinFileEditorClassifications classifications;
        private readonly GherkinFileScope gherkinFileScope;

        private GherkinBuffer gherkinBuffer;
        private readonly ITextSnapshot textSnapshot;

        private GherkinFileBlockBuilder currentFileBlockBuilder;
        private GherkinStep currentStep = null;

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

        protected virtual string FeatureTitle { get { return gherkinFileScope.HeaderBlock == null ? null : gherkinFileScope.HeaderBlock.Title; } }
        protected virtual IEnumerable<string> FeatureTags { get { return gherkinFileScope.HeaderBlock == null ? Enumerable.Empty<string>() : gherkinFileScope.HeaderBlock.Tags; } }

        protected GherkinTextBufferParserListenerBase(GherkinDialect gherkinDialect, ITextSnapshot textSnapshot, GherkinFileEditorClassifications classifications)
        {
            this.textSnapshot = textSnapshot;
            this.classifications = classifications;
            gherkinFileScope = new GherkinFileScope(gherkinDialect, textSnapshot);
        }

        public IGherkinFileScope GetResult()
        {
            if (currentFileBlockBuilder != null)
            {
                if (!CurrentFileBlockBuilder.IsComplete)
                {
                    CurrentFileBlockBuilder.SetAsInvalidBlock();
                }

                CloseBlock(textSnapshot.LineCount);
            }

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
            var endLine = span.StartPosition.Line == span.EndPosition.Line ? 
                startLine : textSnapshot.GetLineFromLineNumber(span.EndPosition.Line);
            var startIndex = startLine.Start + span.StartPosition.LinePosition;
            var endLinePosition = span.EndPosition.LinePosition == endLine.Length ? 
                endLine.LengthIncludingLineBreak : span.EndPosition.LinePosition;
            var length = endLine.Start + endLinePosition - startIndex;
            AddClassification(classificationType, startIndex, length);
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
            if (textSpan.StartPosition < textSpan.EndPosition)
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
            while (gherkinBuffer.GetMatchForLine(commentRe, regionEndLine) != null && regionEndLine > gherkinBuffer.LineOffset)
                regionEndLine--;

            // skip empty lines directly before next scenario start (+ comments)
            while (gherkinBuffer.GetMatchForLine(whitespaceOnlyRe, regionEndLine) != null && regionEndLine > gherkinBuffer.LineOffset)
                regionEndLine--;
            return regionEndLine;
        }

        private Action<int> CloseLevel2Outlinings = null;

        private void OnCloseLevel2Outlinings(int regionEndLine)
        {
            if (CloseLevel2Outlinings != null)
            {
                CloseLevel2Outlinings(regionEndLine);
                CloseLevel2Outlinings = null;
            }
        }

        private void CloseBlock(int editorLine)
        {
            var regionStartLine = CurrentFileBlockBuilder.KeywordLine;
            int regionEndLine = CalculateRegionEndLine(editorLine);

            OnCloseLevel2Outlinings(regionEndLine);

            if (CurrentFileBlockBuilder.SupportsOutlining)
            {
                if (regionEndLine > regionStartLine)
                    AddOutline(
                        regionStartLine,
                        regionEndLine,
                        CurrentFileBlockBuilder.FullTitle);
            }

            BuildBlock(CurrentFileBlockBuilder, editorLine - 1);
            CurrentFileBlockBuilder = null;
        }

        protected virtual void BuildBlock(GherkinFileBlockBuilder blockBuilder, int lastLine)
        {
            blockBuilder.Build(gherkinFileScope, lastLine);
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
                currentStep = null;
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
            VisualStudioTracer.Assert(currentFileBlockBuilder != null, "no current file block builder");
        }

        protected virtual void InitializeFirstBlock(GherkinBufferPosition startPosition)
        {
            CreateBlock(startPosition.Line);
        }

        public void Comment(string commentText, GherkinBufferSpan commentSpan)
        {
            ColorizeSpan(commentSpan, classifications.Comment);
        }

        public virtual void Feature(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            CurrentFileBlockBuilder.SetMainData(typeof(IHeaderBlock), headerSpan.StartPosition.Line, keyword, name);

            ColorizeKeywordLine(keyword, headerSpan, classifications.FeatureTitle);
            ColorizeSpan(descriptionSpan, classifications.Description);
        }

        public virtual void Background(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            NewBlock(headerSpan.StartPosition.Line);
            CurrentFileBlockBuilder.SetMainData(typeof(IBackgroundBlock), headerSpan.StartPosition.Line, keyword, name);
            currentStep = null;

            RegisterKeyword(keyword, headerSpan);
            ColorizeSpan(descriptionSpan, classifications.Description);
        }

        public void Examples(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            var editorLine = headerSpan.StartPosition.Line;
            OnCloseLevel2Outlinings(CalculateRegionEndLine(editorLine));

            RegisterKeyword(keyword, headerSpan);
            ColorizeSpan(descriptionSpan, classifications.Description);

            ScenarioOutlineExampleSet exampleSet = new ScenarioOutlineExampleSet(keyword, name, 
                editorLine - CurrentFileBlockBuilder.KeywordLine);
            CurrentFileBlockBuilder.ExampleSets.Add(exampleSet);

            CloseLevel2Outlinings += regionEndLine =>
                                         {
                                             if (regionEndLine > editorLine)
                                                 AddOutline(
                                                     editorLine,
                                                     regionEndLine,
                                                     exampleSet.FullTitle());
                                         };
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
            CurrentFileBlockBuilder.Tags.Add(name.TrimStart('@'));
        }

        public void ScenarioTag(string name, GherkinBufferSpan tagSpan)
        {
            EnsureNewScenario(tagSpan.StartPosition.Line);
            CurrentFileBlockBuilder.Tags.Add(name.TrimStart('@'));
            ColorizeSpan(tagSpan, classifications.Tag);
        }

        public void ExamplesTag(string name, GherkinBufferSpan tagSpan)
        {
            ColorizeSpan(tagSpan, classifications.Tag);
        }

        private static readonly Regex placeholderRe = new Regex(@"\<.*?\>");
        public void Step(string keyword, StepKeyword stepKeyword, Parser.Gherkin.ScenarioBlock scenarioBlock, string text, GherkinBufferSpan stepSpan)
        {
            ColorizeKeywordLine(keyword, stepSpan, classifications.StepText);

            if (CurrentFileBlockBuilder.BlockType == typeof(IScenarioOutlineBlock))
            {
                var matches = placeholderRe.Matches(text);
                foreach (Match match in matches)
                    ColorizeLinePart(match.Value, stepSpan, classifications.Placeholder);
            }

            var editorLine = stepSpan.StartPosition.Line;
            var tags = FeatureTags.Concat(CurrentFileBlockBuilder.Tags).Distinct();
            var stepScope = new StepScope(FeatureTitle, CurrentFileBlockBuilder.BlockType == typeof(IBackgroundBlock) ? null : CurrentFileBlockBuilder.Title, tags.ToArray());

            currentStep = new GherkinStep((BindingType)scenarioBlock, (StepDefinitionKeyword)stepKeyword, text, stepScope, keyword, editorLine - CurrentFileBlockBuilder.KeywordLine);
            CurrentFileBlockBuilder.Steps.Add(currentStep);
        }

        public void TableHeader(string[] cells, GherkinBufferSpan rowSpan, GherkinBufferSpan[] cellSpans)
        {
            foreach (var cellSpan in cellSpans)
            {
                ColorizeSpan(cellSpan, classifications.TableHeader);
            }

            if (currentStep != null)
            {
                try
                {
                    currentStep.TableArgument = new Table(cells);
                }
                catch(Exception)
                {
                    //TODO: shall we mark it as error?
                }
            }
            //TODO: register outline example
        }

        public void TableRow(string[] cells, GherkinBufferSpan rowSpan, GherkinBufferSpan[] cellSpans)
        {
            foreach (var cellSpan in cellSpans)
            {
                ColorizeSpan(cellSpan, classifications.TableCell);
            }
            if (currentStep != null)
            {
                try
                {
                    currentStep.TableArgument.AddRow(cells);
                }
                catch (Exception)
                {
                    //TODO: shall we mark it as error?
                }
            }
            //TODO: register outline example
        }

        public void MultilineText(string text, GherkinBufferSpan textSpan)
        {
            ColorizeSpan(textSpan, classifications.MultilineText);

            if (currentStep != null)
            {
                currentStep.MultilineTextArgument = text;
            }
            else
            {
                //TODO: shall we mark it as error?
            }
        }

        public void EOF(GherkinBufferPosition eofPosition)
        {
            //nop;
        }

        public void Error(string message, GherkinBufferPosition errorPosition, Exception exception)
        {
            CurrentFileBlockBuilder.Errors.Add(new ErrorInfo(message, errorPosition.Line, errorPosition.LinePosition, exception));
        }
    }
}