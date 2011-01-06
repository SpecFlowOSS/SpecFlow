using System;
using System.Collections.Generic;
using gherkin;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class GherkinFileScope : IGherkinFileScope
    {
        public I18n LanguageService { get; set; }
        public ITextSnapshot TextSnapshot { get; set; }

        public IHeaderBlock HeaderBlock { get; set; }
        public IBackgroundBlock BackgroundBlock { get; set; }
        public List<IScenarioBlock> ScenarioBlocks { get; private set; }

        IEnumerable<IScenarioBlock> IGherkinFileScope.ScenarioBlocks { get { return ScenarioBlocks; } }

        public GherkinFileScope()
        {
            ScenarioBlocks = new List<IScenarioBlock>();
        }
    }

    internal class GherkinStep : IGherkinStep
    {
        public string Keyword { get; set; }
        public string Text { get; set; }
        public int BlockRelativeLine { get; set; }
        public BindingStatus BindingStatus { get; set; }
    }

    internal abstract class GherkinFileBlock : IGherkinFileBlock
    {
        public string Keyword { get; set; }
        public string Title { get; private set; }
        public int KeywordLine { get; private set; }
        public int BlockRelativeStartLine { get; private set; }
        public IEnumerable<ClassificationSpan> ClassificationSpans { get; private set; }
        public IEnumerable<ITagSpan<IOutliningRegionTag>> OutliningRegions { get; private set; }
        public IEnumerable<IErrorInfoTODO> Errors { get; private set; }

        protected GherkinFileBlock(string keyword, string title, int keywordLine, int blockRelativeStartLine, IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<IErrorInfoTODO> errors)
        {
            Keyword = keyword;
            Title = title;
            KeywordLine = keywordLine;
            BlockRelativeStartLine = blockRelativeStartLine;
            ClassificationSpans = classificationSpans;
            OutliningRegions = outliningRegions;
            Errors = errors;
        }
    }

    internal class HeaderBlock : GherkinFileBlock, IHeaderBlock
    {
        public HeaderBlock(string keyword, string title, int keywordLine, int blockRelativeStartLine, IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<IErrorInfoTODO> errors) 
            : base(keyword, title, keywordLine, blockRelativeStartLine, classificationSpans, outliningRegions, errors)
        {
        }
    }

    internal abstract class GherkinFileBlockWithSteps : GherkinFileBlock
    {
        public IEnumerable<IGherkinStep> Steps { get; private set; }
        
        protected GherkinFileBlockWithSteps(string keyword, string title, int keywordLine, int blockRelativeStartLine, IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<IErrorInfoTODO> errors, IEnumerable<IGherkinStep> steps)
            : base(keyword, title, keywordLine, blockRelativeStartLine, classificationSpans, outliningRegions, errors)
        {
            Steps = steps;
        }
    }

    internal class BackgroundBlock : GherkinFileBlockWithSteps, IBackgroundBlock
    {
        public BackgroundBlock(string keyword, string title, int keywordLine, int blockRelativeStartLine, IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<IErrorInfoTODO> errors, IEnumerable<IGherkinStep> steps) 
            : base(keyword, title, keywordLine, blockRelativeStartLine, classificationSpans, outliningRegions, errors, steps)
        {
        }
    }

    internal class ScenarioBlock : GherkinFileBlockWithSteps, IScenarioBlock
    {
        public ScenarioBlock(string keyword, string title, int keywordLine, int blockRelativeStartLine, IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<IErrorInfoTODO> errors, IEnumerable<IGherkinStep> steps)
            : base(keyword, title, keywordLine, blockRelativeStartLine, classificationSpans, outliningRegions, errors, steps)
        {
        }
    }

    internal class ScenarioOutlineBlock : ScenarioBlock, IScenarioOutlineBlock
    {
        public IEnumerable<IScenarouOutlineExampleSet> ExampleSets { get; private set; }

        public ScenarioOutlineBlock(string keyword, string title, int keywordLine, int blockRelativeStartLine, IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<IErrorInfoTODO> errors, IEnumerable<IGherkinStep> steps, IEnumerable<IScenarouOutlineExampleSet> exampleSets) : base(keyword, title, keywordLine, blockRelativeStartLine, classificationSpans, outliningRegions, errors, steps)
        {
            ExampleSets = exampleSets;
        }
    }
}