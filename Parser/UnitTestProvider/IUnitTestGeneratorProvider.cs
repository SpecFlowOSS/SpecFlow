using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.UnitTestProvider
{
    public interface IUnitTestGeneratorProvider
    {
        void SetTestFixture(CodeTypeDeclaration typeDeclaration, string title, string description);
        void SetTestFixtureCategories(CodeTypeDeclaration typeDeclaration, IEnumerable<string> categories);
        void SetTest(CodeMemberMethod memberMethod, string title);
        void SetTestCategories(CodeMemberMethod memberMethod, IEnumerable<string> categories);
        void SetTestSetup(CodeMemberMethod memberMethod);
        void SetTestFixtureSetup(CodeMemberMethod memberMethod);
        void SetTestFixtureTearDown(CodeMemberMethod memberMethod);
        void SetTestTearDown(CodeMemberMethod memberMethod);
        void SetIgnore(CodeTypeMember codeTypeMember);
    }
}