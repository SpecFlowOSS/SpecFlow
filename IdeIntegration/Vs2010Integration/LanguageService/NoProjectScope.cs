using System;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class NoProjectScope : IProjectScope
    {
        public GherkinTextBufferParser GherkinTextBufferParser { get; private set; }
        public GherkinFileEditorClassifications Classifications { get; private set; }
        public GherkinProcessingScheduler GherkinProcessingScheduler { get; private set; }
        public SpecFlowProjectConfiguration SpecFlowProjectConfiguration { get; private set; }
        public GherkinDialectServices GherkinDialectServices { get; private set; }

        public event EventHandler SpecFlowProjectConfigurationChanged;
        public event EventHandler GherkinDialectServicesChanged;

        public GherkinScopeAnalyzer GherkinScopeAnalyzer
        {
            get { return null; }
        }

        public NoProjectScope(GherkinFileEditorClassifications classifications, IVisualStudioTracer visualStudioTracer)
        {
            GherkinTextBufferParser = new GherkinTextBufferParser(this, visualStudioTracer);
            GherkinProcessingScheduler = new GherkinProcessingScheduler(visualStudioTracer);
            SpecFlowProjectConfiguration = new SpecFlowProjectConfiguration();
            GherkinDialectServices = new GherkinDialectServices(SpecFlowProjectConfiguration.GeneratorConfiguration.FeatureLanguage); 
            Classifications = classifications;
        }

        public void Dispose()
        {
            //nop
        }
    }
}