using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    public interface IStepDefinitionSkeletonProvider
    {
        string GetStepDefinitionSkeleton(StepInstance stepInstance);
        string GetBindingClassSkeleton(string stepDefinitions);
    }

    public interface IStepDefinitionSkeletonProvider2
    {
        string GetBindingClassSkeleton(ProgrammingLanguage language, StepInstance[] stepInstances, string namespaceName, string className, StepDefinitionSkeletonStyle style);
    }
}