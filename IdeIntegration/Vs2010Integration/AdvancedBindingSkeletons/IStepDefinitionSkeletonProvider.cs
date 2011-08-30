using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Vs2010Integration.AdvancedBindingSkeletons
{
    public interface IStepDefinitionSkeletonProvider
    {
        string GetStepDefinitionSkeleton(StepInstance stepArgs);
        string GetBindingClassSkeleton(List<StepInstance> steps);
        string GetFileSkeleton(List<StepInstance> steps, StepDefSkeletonInfo info);
        string AddStepsToExistingFile(string fileText, List<StepInstance> steps);
    }
}
