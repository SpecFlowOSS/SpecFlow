using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class GherkinFileScope : IGherkinFileScope
    {
        public GherkinDialect GherkinDialect { get; private set; }
        public ITextSnapshot TextSnapshot { get; private set; }

        public IInvalidFileBlock InvalidFileEndingBlock { get; set; }
        public IHeaderBlock HeaderBlock { get; set; }
        public IBackgroundBlock BackgroundBlock { get; set; }
        public List<IScenarioBlock> ScenarioBlocks { get; private set; }

        IEnumerable<IScenarioBlock> IGherkinFileScope.ScenarioBlocks { get { return ScenarioBlocks; } }

        public GherkinFileScope(GherkinDialect gherkinDialect, ITextSnapshot textSnapshot)
        {
            GherkinDialect = gherkinDialect;
            TextSnapshot = textSnapshot;
            ScenarioBlocks = new List<IScenarioBlock>();
        }
    }

    public class GherkinStep : StepInstance, IKeywordLine
    {
        public int BlockRelativeLine { get; set; }
        public BindingStatus BindingStatus { get; set; }

        public GherkinStep(StepDefinitionType stepDefinitionType, StepDefinitionKeyword stepDefinitionKeyword, string stepText, StepContext stepContext, string keyword, int blockRelativeLine)
            : base(stepDefinitionType, stepDefinitionKeyword, keyword, stepText, stepContext)
        {
            BlockRelativeLine = blockRelativeLine;
            BindingStatus = BindingStatus.UnknownBindingStatus;
        }
    }

    internal abstract class GherkinFileBlock : IGherkinFileBlock
    {
        public string Keyword { get; set; }
        public string Title { get; private set; }
        public int KeywordLine { get; private set; }
        public int BlockRelativeStartLine { get; private set; }
        public int BlockRelativeEndLine { get; private set; }
        public int BlockRelativeContentEndLine { get; private set; }
        public IEnumerable<ClassificationSpan> ClassificationSpans { get; private set; }
        public IEnumerable<ITagSpan<IOutliningRegionTag>> OutliningRegions { get; private set; }
        public IEnumerable<ErrorInfo> Errors { get; private set; }

        protected GherkinFileBlock(string keyword, string title, int keywordLine, int blockRelativeStartLine, int blockRelativeEndLine, int blockRelativeContentEndLine, IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<ErrorInfo> errors)
        {
            Keyword = keyword;
            Title = title;
            KeywordLine = keywordLine;
            BlockRelativeStartLine = blockRelativeStartLine;
            BlockRelativeEndLine = blockRelativeEndLine;
            BlockRelativeContentEndLine = blockRelativeContentEndLine;
            ClassificationSpans = classificationSpans;
            OutliningRegions = outliningRegions;
            Errors = errors;
        }
    }

    internal class HeaderBlock : GherkinFileBlock, IHeaderBlock
    {
        public IEnumerable<string> Tags { get; private set; }

        public HeaderBlock(string keyword, string title, int keywordLine, IEnumerable<string> tags, int blockRelativeStartLine, int blockRelativeEndLine, int blockRelativeContentEndLine, IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<ErrorInfo> errors)
            : base(keyword, title, keywordLine, blockRelativeStartLine, blockRelativeEndLine, blockRelativeContentEndLine, classificationSpans, outliningRegions, errors)
        {
            Tags = tags;
        }
    }

    internal class InvalidFileBlock : GherkinFileBlock, IInvalidFileBlock
    {
        public InvalidFileBlock(int startLine, int endLine, params ErrorInfo[] errorInfos)
            : this(startLine, endLine - startLine, endLine - startLine, new ClassificationSpan[0], new ITagSpan<IOutliningRegionTag>[0], errorInfos ?? new ErrorInfo[0])
        {
            
        }

        public InvalidFileBlock(int startLine, int endLine, int blockRelativeContentEndLine, IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<ErrorInfo> errors) 
            : base(null, null, startLine, 0, endLine - startLine, blockRelativeContentEndLine, classificationSpans, outliningRegions, errors)
        {
        }
    }

    internal abstract class GherkinFileBlockWithSteps : GherkinFileBlock
    {
        public IEnumerable<GherkinStep> Steps { get; private set; }

        protected GherkinFileBlockWithSteps(string keyword, string title, int keywordLine, int blockRelativeStartLine, int blockRelativeEndLine, int blockRelativeContentEndLine, 
            IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<ErrorInfo> errors, IEnumerable<GherkinStep> steps)
            : base(keyword, title, keywordLine, blockRelativeStartLine, blockRelativeEndLine, blockRelativeContentEndLine, classificationSpans, outliningRegions, errors)
        {
            Steps = steps;
        }
    }

    internal class BackgroundBlock : GherkinFileBlockWithSteps, IBackgroundBlock
    {
        public BackgroundBlock(string keyword, string title, int keywordLine, int blockRelativeStartLine, int blockRelativeEndLine, int blockRelativeContentEndLine, 
            IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<ErrorInfo> errors, IEnumerable<GherkinStep> steps) 
            : base(keyword, title, keywordLine, blockRelativeStartLine, blockRelativeEndLine, blockRelativeContentEndLine, classificationSpans, outliningRegions, errors, steps)
        {
        }
    }

    internal class ScenarioBlock : GherkinFileBlockWithSteps, IScenarioBlock
    {
        public ScenarioBlock(string keyword, string title, int keywordLine, int blockRelativeStartLine, int blockRelativeEndLine, int blockRelativeContentEndLine, 
            IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<ErrorInfo> errors, IEnumerable<GherkinStep> steps)
            : base(keyword, title, keywordLine, blockRelativeStartLine, blockRelativeEndLine, blockRelativeContentEndLine, classificationSpans, outliningRegions, errors, steps)
        {
        }
    }

    internal class ScenarioOutlineBlock : ScenarioBlock, IScenarioOutlineBlock
    {
        public IEnumerable<IScenarioOutlineExampleSet> ExampleSets { get; private set; }

        public ScenarioOutlineBlock(string keyword, string title, int keywordLine, int blockRelativeStartLine, int blockRelativeEndLine, int blockRelativeContentEndLine, 
            IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<ErrorInfo> errors, IEnumerable<GherkinStep> steps, IEnumerable<IScenarioOutlineExampleSet> exampleSets) 
            : base(keyword, title, keywordLine, blockRelativeStartLine, blockRelativeEndLine, blockRelativeContentEndLine, classificationSpans, outliningRegions, errors, steps)
        {
            ExampleSets = exampleSets;
        }
    }

    internal abstract class KeywordLine : IKeywordLine
    {
        public string Keyword { get; private set; }
        public string Text { get; private set; }
        public int BlockRelativeLine { get; private set; }

        protected KeywordLine(string keyword, string text, int blockRelativeLine)
        {
            Keyword = keyword;
            Text = text;
            BlockRelativeLine = blockRelativeLine;
        }
    }

    internal class ScenarioOutlineExampleSet : KeywordLine, IScenarioOutlineExampleSet
    {
        public Table ExamplesTable { get; set; }

        public ScenarioOutlineExampleSet(string keyword, string text, int blockRelativeLine) : base(keyword, text, blockRelativeLine)
        {
        }
    }
}