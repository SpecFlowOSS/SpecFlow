using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Generation;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator
{
    internal partial class DefaultDependencyProvider
    {
        partial void RegisterUnitTestGeneratorProviders(ObjectContainer container);

        public virtual void RegisterDefaults(ObjectContainer container)
        {
            container.RegisterTypeAs<FileSystem, IFileSystem>();

            container.RegisterTypeAs<GeneratorConfigurationProvider, IGeneratorConfigurationProvider>();
            container.RegisterTypeAs<InProcGeneratorInfoProvider, IGeneratorInfoProvider>();
            container.RegisterTypeAs<TestGenerator, ITestGenerator>();
            container.RegisterTypeAs<TestHeaderWriter, ITestHeaderWriter>();
            container.RegisterTypeAs<TestUpToDateChecker, ITestUpToDateChecker>();

            container.RegisterTypeAs<GeneratorPluginLoader, IGeneratorPluginLoader>();
            container.RegisterTypeAs<DefaultListener, ITraceListener>();

            container.RegisterTypeAs<UnitTestFeatureGenerator, UnitTestFeatureGenerator>();
            container.RegisterTypeAs<FeatureGeneratorRegistry, IFeatureGeneratorRegistry>();
            container.RegisterTypeAs<UnitTestFeatureGeneratorProvider, IFeatureGeneratorProvider>("default");
            container.RegisterTypeAs<TagFilterMatcher, ITagFilterMatcher>();

            container.RegisterTypeAs<DecoratorRegistry, IDecoratorRegistry>();
            container.RegisterTypeAs<ParallelizeDecorator, ITestClassDecorator>("parallelize");

            container.RegisterInstanceAs(GenerationTargetLanguage.CreateCodeDomHelper(GenerationTargetLanguage.CSharp), GenerationTargetLanguage.CSharp, dispose: true);
            container.RegisterInstanceAs(GenerationTargetLanguage.CreateCodeDomHelper(GenerationTargetLanguage.VB), GenerationTargetLanguage.VB, dispose: true);

            container.RegisterTypeAs<ConfigurationLoader, IConfigurationLoader>();

            container.RegisterTypeAs<SpecFlowGherkinParserFactory, IGherkinParserFactory>();
            
            container.RegisterTypeAs<SpecFlowJsonLocator, ISpecFlowJsonLocator>();

            RegisterUnitTestGeneratorProviders(container);
        }
    }
}
