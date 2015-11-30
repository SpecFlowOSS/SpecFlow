using System.CodeDom;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class CodedUiGeneratorProvider : MsTest2010GeneratorProvider
    {
        protected const string CODEDUI_TESTFIXTURE_ATTR = "Microsoft.VisualStudio.TestTools.UITesting.CodedUITestAttribute";

        public CodedUiGeneratorProvider(CodeDomHelper codeDomHelper)
            : base(codeDomHelper)
        {
        }

        public override void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod,
            string scenarioTitle)
        {
            base.SetTestMethod(generationContext, testMethod, scenarioTitle);

            foreach (CodeAttributeDeclaration declaration in generationContext.TestClass.CustomAttributes)
            {
                if (declaration.Name == TESTFIXTURE_ATTR)
                {
                    generationContext.TestClass.CustomAttributes.Remove(declaration);
                    break;
                }
            }

            generationContext.TestClass.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(CODEDUI_TESTFIXTURE_ATTR)));
        }
    }
}
