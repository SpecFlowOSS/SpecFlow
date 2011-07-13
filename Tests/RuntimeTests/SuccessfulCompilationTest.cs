using System;
using System.Linq;
using NUnit.Framework;
using ParserTests;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class SuccessfulCompilationTest : ExecutionTestBase
    {
        [Test, TestCaseSource(typeof(TestFileHelper), "GetTestFiles")]
        public void CanCompileForFile(string fileName)
        {
            ExecuteForFile(fileName);
        }

        protected override void ExecuteTests(object test, Feature feature)
        {
            //nop
        }
    }
}