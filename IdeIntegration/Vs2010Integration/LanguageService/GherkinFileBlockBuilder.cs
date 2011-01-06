using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class GherkinFileBlockBuilder
    {
        public List<ClassificationSpan> ClassificationSpans { get; private set; }
        public List<ITagSpan<IOutliningRegionTag>> OutliningRegions { get; private set; }
        public List<ErrorInfo> Errors { get; private set; }
        public List<IGherkinStep> Steps { get; private set; }
        public List<IScenarouOutlineExampleSet> ExampleSets { get; private set; }

        public int StartLine { get; private set; }

        public Type BlockType { get; private set; }
        public int KeywordLine { get; private set; }
        public string Keyword { get; private set; }
        public string Title { get; private set; }

        public string FullTitle
        {
            get { return Keyword + Title; }
        }

        public bool IsComplete
        {
            get { return BlockType != null; }
        }

        public int BlockRelativeStartLine
        {
            get
            {
                Debug.Assert(IsComplete);
                return KeywordLine - StartLine;
            }
        }

        public bool SupportsOutlining
        {
            get
            {
                return BlockType == typeof (IBackgroundBlock) || 
                       BlockType == typeof (IScenarioBlock) ||
                       BlockType == typeof (IScenarioOutlineBlock);
            }
        }

        public GherkinFileBlockBuilder(int startLine)
        {
            ClassificationSpans = new List<ClassificationSpan>();
            OutliningRegions = new List<ITagSpan<IOutliningRegionTag>>();
            Errors = new List<ErrorInfo>();
            Steps = new List<IGherkinStep>();
            ExampleSets = new List<IScenarouOutlineExampleSet>();

            StartLine = startLine;
        }

        public void Build(GherkinFileScope gherkinFileEditorInfo)
        {
            Debug.Assert(IsComplete);

            if (BlockType == typeof(IHeaderBlock))
            {
                Debug.Assert(gherkinFileEditorInfo.HeaderBlock == null);
                gherkinFileEditorInfo.HeaderBlock =
                    new HeaderBlock(Keyword, Title, KeywordLine, BlockRelativeStartLine, ClassificationSpans.ToArray(), OutliningRegions.ToArray(), Errors.ToArray());
            }
            else if (BlockType == typeof(IBackgroundBlock))
            {
                Debug.Assert(gherkinFileEditorInfo.BackgroundBlock == null);
                gherkinFileEditorInfo.BackgroundBlock =
                    new BackgroundBlock(Keyword, Title, KeywordLine, BlockRelativeStartLine, ClassificationSpans.ToArray(), OutliningRegions.ToArray(), Errors.ToArray(), Steps.ToArray());
            }
            else if (BlockType == typeof(IScenarioBlock))
            {
                var scenarioBlock = new ScenarioBlock(Keyword, Title, KeywordLine, BlockRelativeStartLine, ClassificationSpans.ToArray(), OutliningRegions.ToArray(), Errors.ToArray(), Steps.ToArray());
                gherkinFileEditorInfo.ScenarioBlocks.Add(scenarioBlock);
            }
            else if (BlockType == typeof(IScenarioOutlineBlock))
            {
                var scenarioBlock = new ScenarioOutlineBlock(Keyword, Title, KeywordLine, BlockRelativeStartLine, ClassificationSpans.ToArray(), OutliningRegions.ToArray(), Errors.ToArray(), Steps.ToArray(), ExampleSets.ToArray());
                gherkinFileEditorInfo.ScenarioBlocks.Add(scenarioBlock);
            }
            else
            {
                throw new NotSupportedException("Block type not supported: " + BlockType);
            }
        }

        public void SetMainData(Type blockType, int keywordLine, string keyword, string title)
        {
            KeywordLine = keywordLine;
            Title = title;
            Keyword = keyword;
            BlockType = blockType;
        }
    }
}