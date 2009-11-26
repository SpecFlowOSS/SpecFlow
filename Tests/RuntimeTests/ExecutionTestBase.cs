using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.CSharp;
using NUnit.Framework;
using ParserTests;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public abstract class ExecutionTestBase
    {
        [Test]
        public void CanExecuteFromFiles()
        {
            foreach (var testFile in TestFileHelper.GetTestFiles())
            {
                CanGenerateFromFile(testFile);
            }
        }

        [Test]
        public void CanExecuteSimpleFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "simple.feature"));
        }

        [Test]
        public void CanExecuteGermanFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "german.feature"));
        }

        [Test]
        public void CanExecuteFrenchFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "french.feature"));
        }

        [Test]
        public void CanExecuteHungarianFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "hungarian.feature"));
        }

        [Test]
        public void CanExecuteCommentsFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "comments.feature"));
        }

        [Test]
        public void CanExecuteFeatureheaderFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "featureheader.feature"));
        }

        [Test]
        public void CanExecuteTagsFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "tags.feature"));
        }

        [Test]
        public void CanExecutebackgroundFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "background.feature"));
        }

        [Test]
        public void CanExecutebackgroundWithTitleFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "background_withtitle.feature"));
        }

        [Test]
        public void CanExecuteWhitespacesFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "whitespaces.feature"));
        }

        [Test]
        public void CanExecuteGivenWhenThenDuplicationFeatureFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "givenwhenthenduplication.feature"));
        }

        [Test]
        public void CanExecuteButFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "but.feature"));
        }

        [Test]
        public void CanExecuteMultilinetitleFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "multilinetitle.feature"));
        }

        [Test]
        public void CanExecuteMultilineargumentFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "multilineargument.feature"));
        }

        [Test]
        public void CanExecuteTableargumentFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "tableargument.feature"));
        }

        [Test]
        public void CanExecuteScneriooutlineFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "scenariooutline.feature"));
        }

        [Test]
        public void CanExecuteMixedGWTFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "mixedgivenwhenthen.feature"));
        }


        [Test, TestCaseSource(typeof(TestFileHelper), "GetTestFiles")]
        public void CanGenerateFromFile(string fileName)
        {
            Console.WriteLine(fileName);
            SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo("en-US"));
            using (var reader = new StreamReader(fileName))
            {
                Feature feature = parser.Parse(reader);
                Assert.IsNotNull(feature);

                ExecuteTests(feature, fileName);
            }
        }

        private void ExecuteTests(Feature feature, string fileName)
        {
            object test = CompileAndCreateTest(fileName, feature);

            ExecuteTests(test, feature);
        }

        protected abstract void ExecuteTests(object test, Feature feature);

        private object CompileAndCreateTest(string fileName, Feature feature)
        {
            string className = Path.GetFileNameWithoutExtension(fileName);
            const string targetNamespace = "Target.Namespace";
            SpecFlowUnitTestConverter converter = new SpecFlowUnitTestConverter(new NUnitTestConverter(), true);
            var codeNamespace = converter.GenerateUnitTestFixture(feature, className, targetNamespace);
            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(codeNamespace);

            Dictionary<string, string> providerOptions = new Dictionary<string, string>();
            providerOptions["CompilerVersion"] = "v3.5";

            CSharpCodeProvider codeProvider = new CSharpCodeProvider(providerOptions);

            CompilerParameters compilerParameters = new CompilerParameters();
            compilerParameters.GenerateInMemory = true;
            compilerParameters.TempFiles.KeepFiles = true;

            compilerParameters.ReferencedAssemblies.Add(
                TestFileHelper.GetAssemblyPath(typeof (TestAttribute))); //NUnit
            compilerParameters.ReferencedAssemblies.Add(
                TestFileHelper.GetAssemblyPath(typeof (ITestRunner))); //TechTalk.SpecFlow

            var results = codeProvider.CompileAssemblyFromDom(compilerParameters, compileUnit);

            if (results.NativeCompilerReturnValue != 0)
                throw new InvalidOperationException("Test cannot be compiled: " + 
                                                    string.Join(Environment.NewLine, results.Errors.Cast<CompilerError>().Select(ce => ce.ToString()).ToArray()));

            Type testType = results.CompiledAssembly.GetType(targetNamespace + "." + className, true);
            return Activator.CreateInstance(testType);
        }
    }
}
