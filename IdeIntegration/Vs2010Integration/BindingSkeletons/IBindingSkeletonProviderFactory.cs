using System;
using System.ComponentModel.Composition;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    public interface IBindingSkeletonProviderFactory
    {
        IStepDefinitionSkeletonProvider GetProvider(ProgrammingLanguage targetLanguage, GherkinDialect gherkinDialect);
    }

    [Export(typeof(IBindingSkeletonProviderFactory))]
    internal class BindingSkeletonProviderFactory : IBindingSkeletonProviderFactory
    {
        public IStepDefinitionSkeletonProvider GetProvider(ProgrammingLanguage targetLanguage, GherkinDialect gherkinDialect)
        {
            switch (targetLanguage)
            {
                case ProgrammingLanguage.CSharp:
                    return new StepDefinitionSkeletonProviderCS(gherkinDialect);
                case ProgrammingLanguage.VB:
                    return new StepDefinitionSkeletonProviderVB(gherkinDialect);
                default:
                    return new StepDefinitionSkeletonProviderCS(gherkinDialect);
            }
        }
    }
}