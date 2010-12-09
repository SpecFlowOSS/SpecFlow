using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using gherkin;
using Microsoft.VisualStudio.Text;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public interface IGherkinFileBlock
    {
        
    }

    public interface IHeaderBlock : IGherkinFileBlock
    {
        
    }

    public interface IBackgroundBlock : IGherkinFileBlock
    {
        
    }

    public interface IScenarioBlock : IGherkinFileBlock
    {
        
    }

    public interface IScenarouOutlineBlock : IScenarioBlock
    {
        
    }

    public class ShiftedBlock<T> : IGherkinFileBlock where T : IGherkinFileBlock
    {
        protected T baseBlock;
        public int LineShift { get; set; }
    }

    public class ShiftedScenarioBlock : ShiftedBlock<IScenarioBlock>, IScenarioBlock
    {
        
    }

    public class ShiftedScenarioOutlineBlock : ShiftedBlock<IScenarouOutlineBlock>, IScenarouOutlineBlock
    {
        
    }

    public class GherkinStep
    {
        public int BlockRelativeLine { get; set; }
        public BindingStatus BindingStatus { get; set; }
    }

    public enum BindingStatusKind
    {
        Unknown,
        Unbound,
        Valid,
        Invalid
    }

    public class BindingStatus
    {
        public readonly static BindingStatus UnknownBindingStatus = new BindingStatus(BindingStatusKind.Unknown);
        public readonly static BindingStatus UnboundBindingStatus = new BindingStatus(BindingStatusKind.Unbound);

        public BindingStatusKind Kind { get; private set; }

        public BindingStatus(BindingStatusKind kind)
        {
            Kind = kind;
        }
    }

    public class GherkinFileScope
    {
        public I18n LanguageService { get; set; }

        public IHeaderBlock HeaderBlock { get; set; }
        public IBackgroundBlock BackgroundBlock { get; set; }
        public IEnumerable<IScenarioBlock> ScenarioBlocks { get; }

        public IEnumerable<IGherkinFileBlock> AllBlocks { get; }
    }

    public class GherkinFileScopeChange : EventArgs
    {
        public GherkinFileScope GherkinFileScope { get; set; }
        
        public bool LanguageChanged { get; set; }
        public bool EntireScopeChanged { get; set; }

        public IEnumerable<IGherkinFileBlock> ChangedBlocks { get; set; }
        public IEnumerable<IGherkinFileBlock> ShiftedBlocks { get; set; }
    }

    public enum GherkinTextBufferChangeType
    {
        EntireFile,
        SingleLine,
        MultiLine
    }

    public class GherkinTextBufferChange
    {
        public GherkinTextBufferChangeType Type { get; set; }

        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }

        public int LineCountDelta { get; set; }
        public int PositionDelta { get; set; }

        public ITextSnapshot ResultTextSnapshot { get; set; }
    }

    internal class ParsingScheduler
    {
        public void EnqueueParsingRequest(GherkinTextBufferChange change)
        {
            throw new NotImplementedException();
        }
    }

    public class GherkinTextBufferParser
    {
        public GherkinFileScopeChange Parse(GherkinTextBufferChange change, GherkinFileScope previousScope = null)
        {
            throw new NotImplementedException();
        }
    }

    public class GherkinScopeAnalyzer
    {
        public GherkinFileScopeChange Analyze(GherkinFileScopeChange change, GherkinFileScope previousScope = null)
        {
            throw new NotImplementedException();
        }
    }

    public class ProjectScopeFactory
    {
        public ProjectScope GetProjectScope(Project project)
        {
//            if (project == null)
//                return CreateNoProjectScope();
//
//            Events2 events2 = project.DTE.Events as Events2;
//            if (events2 == null)
//                return CreateNoProjectScope();
//
//            events2.
//
            return new ProjectScope();
        }

        private ProjectScope CreateNoProjectScope()
        {
            throw new NotImplementedException();
        }
    }

    public class ProjectScope
    {
        
    }

    /// <summary>
    /// Class controlling all Gherkin (feature file) related operation in the Visual Studio editor for a given file.
    /// </summary>
    public class GherkinLanguageService
    {
        /// <summary>
        /// Notifies the subscribers about a change in the (parsed) file scope.
        /// </summary>
        public event EventHandler<GherkinFileScopeChange> FileScopeChanged;

        /// <summary>
        /// Registers a change in the text buffer for the language service. The processing of the change is asynchronous, so it does not block the caller.
        /// </summary>
        public void TextBufferChanged(GherkinTextBufferChange change)
        {
            throw new NotImplementedException();
        }

        public GherkinFileScope GetFileScope(bool waitForLatest = false)
        {
            throw new NotImplementedException();
        }
    }
}
