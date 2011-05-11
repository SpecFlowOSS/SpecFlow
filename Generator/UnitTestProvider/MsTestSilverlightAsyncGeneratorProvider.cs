using System.CodeDom;
using System.Linq;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class MsTestSilverlightAsyncGeneratorProvider : MsTestSilverlightGeneratorProvider
    {
        public override void SetTestFixture(CodeTypeDeclaration typeDeclaration, string title, string description)
        {
            base.SetTestFixture(typeDeclaration, title, description);

            typeDeclaration.BaseTypes.Add(new CodeTypeReference("Microsoft.Silverlight.Testing.SilverlightTest"));
            typeDeclaration.BaseTypes.Add(new CodeTypeReference("TechTalk.SpecFlow.IAsyncFeature"));
            typeDeclaration.BaseTypes.Add(new CodeTypeReference("TechTalk.SpecFlow.AsyncContextProvider.ISilverlightTestInstance"));
        }

        public override void SetTest(CodeMemberMethod memberMethod, string title)
        {
            base.SetTest(memberMethod, title);

            memberMethod.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("Microsoft.Silverlight.Testing.AsynchronousAttribute")));
        }

        public override void FinalizeTestClass(CodeNamespace codeNameSpace)
        {
            base.FinalizeTestClass(codeNameSpace);

            var methods = from codeTypeMember in codeNameSpace.Types[0].Members.Cast<CodeTypeMember>()
                          where codeTypeMember.Name == "ScenarioSetup"
                          select (CodeMemberMethod)codeTypeMember;

            var scenarioSetupMethod = methods.First();

            var silverlightTestAsyncContext = new CodeObjectCreateExpression("TechTalk.SpecFlow.AsyncContextProvider.SilverlightTestAsyncContext", new CodeThisReferenceExpression());

            var asyncContext = new CodeTypeReferenceExpression("AsyncContext");
            var registerAsyncExpression = new CodeMethodInvokeExpression(asyncContext, "Register", silverlightTestAsyncContext);

            scenarioSetupMethod.Statements.Insert(0, new CodeExpressionStatement(registerAsyncExpression));
        }
    }
}