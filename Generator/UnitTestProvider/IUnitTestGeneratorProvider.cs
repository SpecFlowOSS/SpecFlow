using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public interface IUnitTestGeneratorProvider
    {
        bool SupportsRowTests { get; }
        void SetTestFixture(CodeTypeDeclaration typeDeclaration, string title, string description);
        void SetTestFixtureCategories(CodeTypeDeclaration typeDeclaration, IEnumerable<string> categories);
        void SetTest(CodeMemberMethod memberMethod, string title);
        void SetRowTest(CodeMemberMethod memberMethod, string title);
        void SetRow(CodeMemberMethod memberMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored);
        void SetTestCategories(CodeMemberMethod memberMethod, IEnumerable<string> categories);
        void SetTestSetup(CodeMemberMethod memberMethod);
        void SetTestFixtureSetup(CodeMemberMethod memberMethod);
        void SetTestFixtureTearDown(CodeMemberMethod memberMethod);
        void SetTestTearDown(CodeMemberMethod memberMethod);
        void SetIgnore(CodeTypeMember codeTypeMember);
    }
}