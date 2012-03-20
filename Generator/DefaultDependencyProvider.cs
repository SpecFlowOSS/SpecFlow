using BoDi;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator
{
    internal partial class DefaultDependencyProvider
    {
        partial void RegisterUnitTestGeneratorProviders(ObjectContainer container);

        public virtual void RegisterDefaults(ObjectContainer container)
        {
            container.RegisterTypeAs<SpecFlowProjectConfigurationLoader, ISpecFlowProjectConfigurationLoader>();
            container.RegisterTypeAs<InProcGeneratorInfoProvider, IGeneratorInfoProvider>();
            container.RegisterTypeAs<TestGenerator, ITestGenerator>();
            container.RegisterTypeAs<TestHeaderWriter, ITestHeaderWriter>();
            container.RegisterTypeAs<TestUpToDateChecker, ITestUpToDateChecker>();

            container.RegisterTypeAs<SpecFlowUnitTestConverter, ISpecFlowUnitTestConverter>();

            container.RegisterInstanceAs(GenerationTargetLanguage.CreateCodeDomHelper(GenerationTargetLanguage.CSharp), GenerationTargetLanguage.CSharp);
            container.RegisterInstanceAs(GenerationTargetLanguage.CreateCodeDomHelper(GenerationTargetLanguage.VB), GenerationTargetLanguage.VB);

            RegisterUnitTestGeneratorProviders(container);
        }
    }
}