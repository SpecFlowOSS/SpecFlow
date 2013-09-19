using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class GherkinFileBlockBuilder
    {
        public List<ClassificationSpan> ClassificationSpans { get; private set; }
        public List<ITagSpan<IOutliningRegionTag>> OutliningRegions { get; private set; }
        public List<ErrorInfo> Errors { get; private set; }
        public List<GherkinStep> Steps { get; private set; }
        public List<ScenarioOutlineExampleSet> ExampleSets { get; private set; }
        public List<string> Tags { get; private set; }

        public int StartLine { get; private set; }

        public Type BlockType { get; private set; }
        public int KeywordLine { get; private set; }
        public string Keyword { get; private set; }
        public string Title { get; private set; }

        public string FullTitle
        {
            get
            {
                return GherkinFileScopeExtensions.FormatBlockFullTitle(Keyword, Title);
            }
        }

        public bool IsComplete
        {
            get { return BlockType != null; }
        }

        public int BlockRelativeStartLine
        {
            get
            {
                VisualStudioTracer.Assert(IsComplete, "The block builder is not complete");
                return StartLine - KeywordLine;
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
            Steps = new List<GherkinStep>();
            ExampleSets = new List<ScenarioOutlineExampleSet>();
            Tags = new List<string>();

            StartLine = startLine;
        }

        public void Build(GherkinFileScope gherkinFileEditorInfo, int endLine, int contentEndLine)
        {
            VisualStudioTracer.Assert(IsComplete, "The block builder is not complete");
            int blockRelativeEndLine = endLine - KeywordLine;
            int blockRelativeContentEndLine = contentEndLine - KeywordLine;

            if (BlockType == typeof(IInvalidFileBlock))
            {
                VisualStudioTracer.Assert(gherkinFileEditorInfo.InvalidFileEndingBlock == null, "no invalid file block");
                if (gherkinFileEditorInfo.InvalidFileEndingBlock == null)
                    gherkinFileEditorInfo.InvalidFileEndingBlock =
                        new InvalidFileBlock(StartLine, endLine, blockRelativeContentEndLine, ClassificationSpans.ToArray(), OutliningRegions.ToArray(), Errors.ToArray());
            }
            else if (BlockType == typeof(IHeaderBlock))
            {
                VisualStudioTracer.Assert(gherkinFileEditorInfo.HeaderBlock == null, "no header block");
                if (gherkinFileEditorInfo.HeaderBlock == null)
                    gherkinFileEditorInfo.HeaderBlock =
                        new HeaderBlock(Keyword, Title, KeywordLine, Tags.ToArray(), BlockRelativeStartLine, blockRelativeEndLine, blockRelativeContentEndLine, ClassificationSpans.ToArray(), OutliningRegions.ToArray(), Errors.ToArray());
            }
            else if (BlockType == typeof(IBackgroundBlock))
            {
                VisualStudioTracer.Assert(gherkinFileEditorInfo.BackgroundBlock == null, "no background block");
                if (gherkinFileEditorInfo.BackgroundBlock == null)
                    gherkinFileEditorInfo.BackgroundBlock =
                        new BackgroundBlock(Keyword, Title, KeywordLine, BlockRelativeStartLine, blockRelativeEndLine, blockRelativeContentEndLine, ClassificationSpans.ToArray(), OutliningRegions.ToArray(), Errors.ToArray(), Steps.ToArray());
            }
            else if (BlockType == typeof(IScenarioBlock))
            {
                var scenarioBlock = new ScenarioBlock(Keyword, Title, KeywordLine, BlockRelativeStartLine, blockRelativeEndLine, blockRelativeContentEndLine, ClassificationSpans.ToArray(), OutliningRegions.ToArray(), Errors.ToArray(), Steps.ToArray());
                gherkinFileEditorInfo.ScenarioBlocks.Add(scenarioBlock);
            }
            else if (BlockType == typeof(IScenarioOutlineBlock))
            {
                var scenarioBlock = new ScenarioOutlineBlock(Keyword, Title, KeywordLine, BlockRelativeStartLine, blockRelativeEndLine, blockRelativeContentEndLine, ClassificationSpans.ToArray(), OutliningRegions.ToArray(), Errors.ToArray(), Steps.ToArray(), ExampleSets.ToArray());
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

        public void SetAsInvalidBlock()
        {
            BlockType = typeof(IInvalidFileBlock);
        }
    }
}