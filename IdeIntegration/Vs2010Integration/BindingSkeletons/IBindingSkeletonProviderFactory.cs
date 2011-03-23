using System;
using System.ComponentModel.Composition;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    public interface IBindingSkeletonProviderFactory
    {
        IStepDefinitionSkeletonProvider GetProvider(GenerationTargetLanguage targetLanguage, GherkinDialect gherkinDialect);
    }

    [Export(typeof(IBindingSkeletonProviderFactory))]
    internal class BindingSkeletonProviderFactory : IBindingSkeletonProviderFactory
    {
        public IStepDefinitionSkeletonProvider GetProvider(GenerationTargetLanguage targetLanguage, GherkinDialect gherkinDialect)
        {
            switch (targetLanguage)
            {
                case GenerationTargetLanguage.CSharp:
                    return new StepDefinitionSkeletonProviderCS(gherkinDialect);
                case GenerationTargetLanguage.VB:
                    return new StepDefinitionSkeletonProviderVB(gherkinDialect);
                default:
                    return new StepDefinitionSkeletonProviderCS(gherkinDialect);
            }
        }
    }
}