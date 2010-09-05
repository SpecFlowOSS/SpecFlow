using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Tracing
{
    internal interface IStepDefinitionSkeletonProvider
    {
        string GetStepDefinitionSkeleton(StepArgs stepArgs);
        string GetBindingClassSkeleton(string stepDefinitions);
    }
}