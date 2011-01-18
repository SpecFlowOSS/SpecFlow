using System;
using EnvDTE;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public interface IProjectScope : IDisposable
    {
        GherkinTextBufferParser GherkinTextBufferParser { get; }
        GherkinScopeAnalyzer GherkinScopeAnalyzer { get; }
        GherkinDialectServices GherkinDialectServices { get; }
        GherkinFileEditorClassifications Classifications { get; }
        GherkinProcessingScheduler GherkinProcessingScheduler { get; }
        SpecFlowProjectConfiguration SpecFlowProjectConfiguration { get; }
    }

    internal class NoProjectScope : IProjectScope
    {
        public static NoProjectScope Instance = new NoProjectScope();

        #region Implementation of IProjectScope

        public GherkinTextBufferParser GherkinTextBufferParser
        {
            get { throw new NotImplementedException(); }
        }

        public GherkinScopeAnalyzer GherkinScopeAnalyzer
        {
            get { throw new NotImplementedException(); }
        }

        public GherkinDialectServices GherkinDialectServices
        {
            get { throw new NotImplementedException(); }
        }

        public GherkinFileEditorClassifications Classifications
        {
            get { throw new NotImplementedException(); }
        }

        public GherkinProcessingScheduler GherkinProcessingScheduler
        {
            get { throw new NotImplementedException(); }
        }

        public SpecFlowProjectConfiguration SpecFlowProjectConfiguration
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public void Dispose()
        {
            //nop
        }
    }

    public class VsProjectScope : IProjectScope
    {
        private readonly Project project;
        private readonly IVisualStudioTracer visualStudioTracer;
        private readonly GherkinTextBufferParser parser;
        private readonly GherkinScopeAnalyzer analyzer;

        public GherkinFileEditorClassifications Classifications { get; private set; }
        public GherkinProcessingScheduler GherkinProcessingScheduler { get; private set; }
        public SpecFlowProjectConfiguration SpecFlowProjectConfiguration { get; private set; }
        public GherkinDialectServices GherkinDialectServices { get; private set; }

        public Project Project
        {
            get { return project; }
        }

        public IVisualStudioTracer VisualStudioTracer
        {
            get { return visualStudioTracer; }
        }

        public VsProjectScope(Project project, GherkinFileEditorClassifications classifications, IVisualStudioTracer visualStudioTracer)
        {
            Classifications = classifications;
            this.project = project;
            this.visualStudioTracer = visualStudioTracer;
            //TODO: register for file changes, etc.

            parser = new GherkinTextBufferParser(this, visualStudioTracer);
            analyzer = new GherkinScopeAnalyzer(this, visualStudioTracer);
            GherkinProcessingScheduler = new GherkinProcessingScheduler(visualStudioTracer);

            SpecFlowProjectConfiguration = DteProjectReader.LoadSpecFlowConfigurationFromDteProject(project) ?? new SpecFlowProjectConfiguration();
            GherkinDialectServices = new GherkinDialectServices(SpecFlowProjectConfiguration.GeneratorConfiguration.FeatureLanguage);
        }

        public GherkinTextBufferParser GherkinTextBufferParser
        {
            get { return parser; }
        }

        public GherkinScopeAnalyzer GherkinScopeAnalyzer
        {
            get { return analyzer; }
        }

        public void Dispose()
        {
            GherkinProcessingScheduler.Dispose();
        }
    }
}