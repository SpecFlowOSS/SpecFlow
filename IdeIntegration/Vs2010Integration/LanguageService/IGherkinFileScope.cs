using System.Collections.Generic;
using gherkin;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public interface IGherkinFileScope
    {
        I18n LanguageService { get; }
        IHeaderBlock HeaderBlock { get; }
        IBackgroundBlock BackgroundBlock { get; }
        IEnumerable<IScenarioBlock> ScenarioBlocks { get; }

        ITextSnapshot TextSnapshot { get; }
    }

    public interface IErrorInfoTODO
    {

    }

    public class ErrorInfo: IErrorInfoTODO
    {
        
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
        IEnumerable<IErrorInfoTODO> Errors { get; }
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

    public interface IHeaderBlock : IGherkinFileBlock
    {
    }

    public interface IGherkinStep : IKeywordLine
    {
        BindingStatus BindingStatus { get; }
    }

    public interface IBackgroundBlock : IGherkinFileBlock
    {
        IEnumerable<IGherkinStep> Steps { get; }
    }

    public interface IScenarioBlock : IGherkinFileBlock
    {
        IEnumerable<IGherkinStep> Steps { get; }
    }

    public interface IScenarouOutlineExampleSet : IKeywordLine
    {
        IEnumerable<IGherkinStep> Steps { get; }
    }

    public interface IScenarioOutlineBlock : IScenarioBlock
    {
        IEnumerable<IScenarouOutlineExampleSet> ExampleSets { get; }
    }
}