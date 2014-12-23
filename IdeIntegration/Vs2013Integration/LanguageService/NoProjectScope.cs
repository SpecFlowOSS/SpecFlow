using System;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class NoProjectScope : IProjectScope
    {
        public GherkinTextBufferParser GherkinTextBufferParser { get; private set; }
        public GherkinFileEditorClassifications Classifications { get; private set; }
        public IGherkinProcessingScheduler GherkinProcessingScheduler { get; private set; }
        public SpecFlowProjectConfiguration SpecFlowProjectConfiguration { get; private set; }
        public GherkinDialectServices GherkinDialectServices { get; private set; }
        public IIntegrationOptionsProvider IntegrationOptionsProvider { get; private set; }
        public IIdeTracer Tracer { get; private set; }

        public event Action SpecFlowProjectConfigurationChanged { add {} remove {} }
        public event Action GherkinDialectServicesChanged { add { } remove { } }

        public GherkinScopeAnalyzer GherkinScopeAnalyzer
        {
            get { return null; }
        }

        public VsStepSuggestionProvider StepSuggestionProvider
        {
            get { return null; }
        }

        public IStepDefinitionMatchService BindingMatchService
        {
            get { return null; }
        }

        public IGeneratorServices GeneratorServices
        {
            get { return null; }
        }

        public NoProjectScope(GherkinFileEditorClassifications classifications, IVisualStudioTracer visualStudioTracer, IIntegrationOptionsProvider integrationOptionsProvider)
        {
            GherkinTextBufferParser = new GherkinTextBufferParser(this, visualStudioTracer);
            GherkinProcessingScheduler = new GherkinProcessingScheduler(visualStudioTracer, false);
            SpecFlowProjectConfiguration = new SpecFlowProjectConfiguration();
            GherkinDialectServices = new GherkinDialectServices(SpecFlowProjectConfiguration.GeneratorConfiguration.FeatureLanguage); 
            Classifications = classifications;
            IntegrationOptionsProvider = integrationOptionsProvider;
            Tracer = visualStudioTracer;
        }

        public void Dispose()
        {
            //nop
        }
    }
}