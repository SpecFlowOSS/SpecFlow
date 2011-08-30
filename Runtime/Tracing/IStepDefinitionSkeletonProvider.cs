using System.Collections.Generic;
using System.Globalization;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Tracing
{
    internal interface IStepDefinitionSkeletonProvider
    {
        string GetStepDefinitionSkeleton(StepArgs stepArgs);
        string GetBindingClassSkeleton(List<StepArgs> steps);
        string GetFileSkeleton(List<StepArgs> steps, StepDefSkeletonInfo info);
        string AddStepsToExistingFile(string fileText, List<StepArgs> steps);
    }

    public class StepDefSkeletonInfo
    {
        public string SuggestedStepDefName { get; set; }
        public string NamespaceName { get; set; }

        public StepDefSkeletonInfo(string suggestedStepDefName, string namespaceName)
        {
            SuggestedStepDefName = suggestedStepDefName;
            NamespaceName = namespaceName;
        }
    }
}