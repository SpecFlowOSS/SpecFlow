using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Tracing
{
    public interface IStepDefinitionSkeletonProvider
    {
        string GetStepDefinitionSkeleton(StepInstance stepInstance);
        string GetBindingClassSkeleton(string stepDefinitions);
    }
}