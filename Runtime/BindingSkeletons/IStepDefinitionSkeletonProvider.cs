using System.Globalization;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    public interface IStepDefinitionSkeletonProvider
    {
        string GetBindingClassSkeleton(ProgrammingLanguage language, StepInstance[] stepInstances, string namespaceName, string className, StepDefinitionSkeletonStyle style, CultureInfo bindingCulture);
        string GetStepDefinitionSkeleton(ProgrammingLanguage language, StepInstance stepInstance, StepDefinitionSkeletonStyle style, CultureInfo bindingCulture);
    }
}