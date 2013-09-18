using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public interface IGherkinFileScope
    {
        GherkinDialect GherkinDialect { get; }
        IInvalidFileBlock InvalidFileEndingBlock { get; }
        IHeaderBlock HeaderBlock { get; }
        IBackgroundBlock BackgroundBlock { get; }
        IEnumerable<IScenarioBlock> ScenarioBlocks { get; }

        ITextSnapshot TextSnapshot { get; }
    }

    public class ErrorInfo
    {
        public string Message { get; set; }
        public int Line { get; set; }
        public int LinePosition { get; set; }
        public Exception Exception { get; set; }

        public ErrorInfo(string message, int line, int linePosition, Exception exception)
        {
            Message = message;
            Line = line;
            LinePosition = linePosition;
            Exception = exception;
        }
    }

    public interface IGherkinFileBlock
    {
        /// <summary>
        /// The keyword as it was specified in the file, including the tailing colon and space.
        /// </summary>
        string Keyword { get; }

        /// <summary>
        /// The title of the block as a direct concatenation of the <see cref="Keyword"/> in the file (no space trimming in front). 
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The absolute line number (zero-indexed) of the line containing the <see cref="Keyword"/>.
        /// </summary>
        int KeywordLine { get; }

        /// <summary>
        /// A line number relative to <see cref="KeywordLine"/> specifying the first line of the block (can be negative).
        /// </summary>
        int BlockRelativeStartLine { get; }

        /// <summary>
        /// A line number relative to <see cref="KeywordLine"/> specifying the last line of the block (can be zero).
        /// </summary>
        int BlockRelativeEndLine { get; }

        /// <summary>
        /// A line number relative to <see cref="KeywordLine"/> specifying the last line of the block containing important (non-comment) content (can be zero).
        /// </summary>
        int BlockRelativeContentEndLine { get; }

        /// <summary>
        /// The coloring information for this block (null, if no coloring).
        /// </summary>
        IEnumerable<ClassificationSpan> ClassificationSpans { get; }

        /// <summary>
        /// The outlining information for this block (null, if no outlining).
        /// </summary>
        IEnumerable<ITagSpan<IOutliningRegionTag>> OutliningRegions { get; }

        /// <summary>
        /// Any parsing errors in this block.
        /// </summary>
        IEnumerable<ErrorInfo> Errors { get; }
    }

    public interface IHeaderBlock : IGherkinFileBlock
    {
        IEnumerable<string> Tags { get; }
    }

    public interface IStepBlock : IGherkinFileBlock
    {
        IEnumerable<GherkinStep> Steps { get; }
    }

    public interface IBackgroundBlock : IStepBlock
    {
    }

    public interface IInvalidFileBlock : IGherkinFileBlock
    {
    }

    public interface IKeywordLine
    {
        /// <summary>
        /// The keyword as it was specified in the file, including the tailing colon and space.
        /// </summary>
        string Keyword { get; }

        /// <summary>
        /// The text of the line as a direct concatenation of the <see cref="Keyword"/> in the file (no space trimming in front). 
        /// </summary>
        string Text { get; }

        /// <summary>
        /// A line number relative to <see cref="IScenarioBlock.KeywordLine"/> specifying the this line.
        /// </summary>
        int BlockRelativeLine { get; }
    }

    public interface IScenarioBlock : IStepBlock
    {
    }

    public interface IScenarioOutlineExampleSet : IKeywordLine
    {
        ScenarioOutlineExamplesTable ExamplesTable { get; set; }
    }

    public interface IScenarioOutlineBlock : IScenarioBlock
    {
        IEnumerable<IScenarioOutlineExampleSet> ExampleSets { get; }
    }

    public interface ITableWithRowPositions
    {
        void SetBlockRelativePosition(int rowIndex, int blockRelativeLine);
    }

    public class ScenarioOutlineExamplesRow : Dictionary<string, string>
    {
        public int BlockRelativeLine { get; private set; }

        public ScenarioOutlineExamplesRow(TableRow tableRow, int blockRelativeLine) : base(tableRow)
        {
            BlockRelativeLine = blockRelativeLine;
        }
    }

    public class ScenarioOutlineExamplesTable : Table, ITableWithRowPositions
    {
        private readonly Dictionary<int, int> blockRelativeLines = new Dictionary<int, int>();

        public ScenarioOutlineExamplesTable(string[] cells) : base(cells)
        {
        }

        public ScenarioOutlineExamplesRow GetExamplesRow(int rowIndex)
        {
            int blockRelativeLine;
            if (!blockRelativeLines.TryGetValue(rowIndex, out blockRelativeLine))
                blockRelativeLine = -1;
            return new ScenarioOutlineExamplesRow(Rows[rowIndex], blockRelativeLine);
        }

        public void SetBlockRelativePosition(int rowIndex, int blockRelativeLine)
        {
            blockRelativeLines[rowIndex] = blockRelativeLine;
        }

        public ScenarioOutlineExamplesRow FindByBlockRelativeLine(int blockRelativeLine)
        {
            var selectedLines = blockRelativeLines.Where(r2l => r2l.Value == blockRelativeLine).Select(r2l => r2l.Key).ToArray();
            if (selectedLines.Length == 0)
                return null;
            return GetExamplesRow(selectedLines[0]);
        }
    }
}