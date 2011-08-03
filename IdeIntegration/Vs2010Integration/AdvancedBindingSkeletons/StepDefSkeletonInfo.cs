using System.Globalization;

namespace TechTalk.SpecFlow.Vs2010Integration.AdvancedBindingSkeletons
{
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
