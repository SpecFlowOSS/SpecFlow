using System;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public interface IProjectScope : IDisposable
    {
        GherkinTextBufferParser GherkinTextBufferParser { get; }
        GherkinScopeAnalyzer GherkinScopeAnalyzer { get; }
        GherkinDialectServices GherkinDialectServices { get; }
        GherkinFileEditorClassifications Classifications { get; }
        IGherkinProcessingScheduler GherkinProcessingScheduler { get; }
        SpecFlowProjectConfiguration SpecFlowProjectConfiguration { get; }
        VsStepSuggestionProvider StepSuggestionProvider { get; }
        IStepDefinitionMatchService BindingMatchService { get; }
        IIntegrationOptionsProvider IntegrationOptionsProvider { get; }
        IGeneratorServices GeneratorServices { get; }
        IIdeTracer Tracer { get; }

        event Action SpecFlowProjectConfigurationChanged;
        event Action GherkinDialectServicesChanged;
    }
}