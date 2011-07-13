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
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public abstract class ExecutionTestBase
    {
        protected void ExecuteForFile(string fileName)
        {
            Console.WriteLine(fileName);
            SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo("en-US"));
            using (var reader = new StreamReader(fileName))
            {
                Feature feature = parser.Parse(reader, null);
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
            // the row test generation has to be set to false, because our verifications support the old style test generation only
            SpecFlowUnitTestConverter converter = new SpecFlowUnitTestConverter(CreateUnitTestGeneratorProvider(), new CodeDomHelper(CodeDomProviderLanguage.CSharp), new GeneratorConfiguration { AllowRowTests = false, AllowDebugGeneratedFiles = true }); 
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
                TestFileHelper.GetAssemblyPath(typeof (GeneratedCodeAttribute))); //System
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

        protected virtual IUnitTestGeneratorProvider CreateUnitTestGeneratorProvider()
        {
            return new NUnitTestConverter();
        }
    }
}
