using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    public interface IStepDefinitionSkeletonProvider
    {
        string GetStepDefinitionSkeleton(StepInstance stepInstance);
        string GetBindingClassSkeleton(string stepDefinitions);
    }
}