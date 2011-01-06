using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public static class ShiftedGherkinFileBlockExtensions
    {
        public static IGherkinFileBlock Shift(this IGherkinFileBlock fileBlock, int lineShift)
        {
            if (fileBlock == null) throw new ArgumentNullException("fileBlock");

            if (fileBlock is IHeaderBlock)
                return Shift((IHeaderBlock)fileBlock, lineShift);
            if (fileBlock is IBackgroundBlock)
                return Shift((IBackgroundBlock)fileBlock, lineShift);
            if (fileBlock is IScenarioOutlineBlock)
                return Shift((IScenarioOutlineBlock)fileBlock, lineShift);
            if (fileBlock is IScenarioBlock)
                return Shift((IScenarioBlock)fileBlock, lineShift);
            throw new NotSupportedException("block type not supported: " + fileBlock.GetType());
        }

        public static IHeaderBlock Shift(this IHeaderBlock fileBlock, int lineShift)
        {
            if (fileBlock == null) throw new ArgumentNullException("fileBlock");

            UnWrapShiftdFileBlock(ref fileBlock, ref lineShift);
            return new ShiftedHeaderBlock(fileBlock, lineShift);
        }

        public static IBackgroundBlock Shift(this IBackgroundBlock fileBlock, int lineShift)
        {
            if (fileBlock == null) throw new ArgumentNullException("fileBlock");

            UnWrapShiftdFileBlock(ref fileBlock, ref lineShift);
            return new ShiftedBackgroundBlock(fileBlock, lineShift);
        }

        public static IScenarioBlock Shift(this IScenarioBlock fileBlock, int lineShift)
        {
            if (fileBlock == null) throw new ArgumentNullException("fileBlock");

            if (fileBlock is IScenarioOutlineBlock)
                return Shift((IScenarioOutlineBlock)fileBlock, lineShift);

            UnWrapShiftdFileBlock(ref fileBlock, ref lineShift);
            return new ShiftedScenarioBlock(fileBlock, lineShift);
        }

        public static IScenarioOutlineBlock Shift(this IScenarioOutlineBlock fileBlock, int lineShift)
        {
            if (fileBlock == null) throw new ArgumentNullException("fileBlock");

            UnWrapShiftdFileBlock(ref fileBlock, ref lineShift);
            return new ShiftedScenarioOutlineBlock(fileBlock, lineShift);
        }

        private static void UnWrapShiftdFileBlock<T>(ref T fileBlock, ref int lineShift) where T : IGherkinFileBlock
        {
            ShiftedGherkinFileBlock<T> shiftedGherkinFileBlock = fileBlock as ShiftedGherkinFileBlock<T>;
            if (shiftedGherkinFileBlock != null)
            {
                // if it was already shifted, we change the wrapper only
                fileBlock = shiftedGherkinFileBlock.BaseBlock;
                lineShift += shiftedGherkinFileBlock.LineShift;
            }
        }
    }

    internal class ShiftedGherkinFileBlock<T> : IGherkinFileBlock where T : IGherkinFileBlock
    {
        protected T baseBlock;
        public int LineShift { get; set; }

        public T BaseBlock
        {
            get { return baseBlock; }
        }

        public string Keyword
        {
            get { return baseBlock.Keyword; }
        }

        public string Title
        {
            get { return baseBlock.Title; }
        }

        public int KeywordLine
        {
            get { return baseBlock.KeywordLine + LineShift; }
        }

        public int BlockRelativeStartLine
        {
            get { return baseBlock.BlockRelativeStartLine; }
        }

        public IEnumerable<ClassificationSpan> ClassificationSpans
        {
            get { return baseBlock.ClassificationSpans; }
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> OutliningRegions
        {
            get { return baseBlock.OutliningRegions; }
        }

        public IEnumerable<IErrorInfoTODO> Errors
        {
            get { return baseBlock.Errors; }
        }

        public ShiftedGherkinFileBlock(T baseBlock, int lineShift)
        {
            this.baseBlock = baseBlock;
            LineShift = lineShift;
        }
    }

    internal class ShiftedHeaderBlock : ShiftedGherkinFileBlock<IHeaderBlock>, IHeaderBlock
    {
        public ShiftedHeaderBlock(IHeaderBlock baseBlock, int lineShift) : base(baseBlock, lineShift)
        {
        }
    }

    internal class ShiftedBackgroundBlock : ShiftedGherkinFileBlock<IBackgroundBlock>, IBackgroundBlock
    {
        public ShiftedBackgroundBlock(IBackgroundBlock baseBlock, int lineShift) : base(baseBlock, lineShift)
        {
        }

        public IEnumerable<IGherkinStep> Steps
        {
            get { return baseBlock.Steps; }
        }
    }

    internal class ShiftedScenarioBlock : ShiftedGherkinFileBlock<IScenarioBlock>, IScenarioBlock
    {
        public ShiftedScenarioBlock(IScenarioBlock baseBlock, int lineShift) : base(baseBlock, lineShift)
        {
        }

        public IEnumerable<IGherkinStep> Steps
        {
            get { return baseBlock.Steps; }
        }
    }

    internal class ShiftedScenarioOutlineBlock : ShiftedGherkinFileBlock<IScenarioOutlineBlock>, IScenarioOutlineBlock
    {
        public ShiftedScenarioOutlineBlock(IScenarioOutlineBlock baseBlock, int lineShift) : base(baseBlock, lineShift)
        {
        }

        public IEnumerable<IGherkinStep> Steps
        {
            get { return baseBlock.Steps; }
        }

        public IEnumerable<IScenarouOutlineExampleSet> ExampleSets
        {
            get { return baseBlock.ExampleSets; }
        }
    }
}