using System;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.IdeIntegration;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;
using TechTalk.SpecFlow.Vs2010Integration.Options;

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
        VsStepSuggestionProvider StepSuggestionProvider { get; }
        IStepDefinitionMatchService BindingMatchService { get; }
        IIntegrationOptionsProvider IntegrationOptionsProvider { get; }
        IGeneratorServices GeneratorServices { get; }
        IIdeTracer Tracer { get; }

        event Action SpecFlowProjectConfigurationChanged;
        event Action GherkinDialectServicesChanged;
    }
}