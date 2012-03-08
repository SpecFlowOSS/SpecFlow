using System.CodeDom;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class MsTestMolesGeneratorProvider : MsTestGeneratorProvider
    {
        private const string MOLESHOSTTYPE_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.HostType";
        public override void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            base.SetTestMethod(generationContext, testMethod, scenarioTitle);
            CodeDomHelper.AddAttribute(testMethod, MOLESHOSTTYPE_ATTR, "Moles");
        }
    }
}