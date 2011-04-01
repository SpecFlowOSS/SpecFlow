using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using Microsoft.CSharp;
using NUnit.Framework;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using System.CodeDom;
using TechTalk.SpecFlow.Utils;
using TechTalk.SpecFlow.Parser;
using System.Globalization;
using System.IO;
using System.CodeDom.Compiler;

namespace ParserTests
{
    [TestFixture]
    public class SampleTestGeneratorProviderTest
    {
        [Test]
        public void CanGenerateFromFile()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            string fileName = Path.Combine(folder, "scenariooutline.feature");
            SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo("en-US"));
            using (var reader = new StreamReader(fileName))
            {
                Feature feature = parser.Parse(reader, null);
                Assert.IsNotNull(feature);

                var codeDomHelper = new CodeDomHelper(CodeDomProviderLanguage.CSharp);
                var sampleTestGeneratorProvider = new SampleTestGeneratorProvider();
                var converter = new SpecFlowUnitTestConverter(sampleTestGeneratorProvider, codeDomHelper, true, true);

                CodeNamespace code = converter.GenerateUnitTestFixture(feature, "TestClassName", "Target.Namespace");
                using (TextWriter writer = Console.Out)
                {
                    CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                        CodeGeneratorOptions options = new CodeGeneratorOptions();
                    codeProvider.GenerateCodeFromNamespace(code, writer, options);
                }
                Assert.IsNotNull(code);
                Assert.AreEqual(code.Name, sampleTestGeneratorProvider.GetType().Name);
            }
        }
        
        class SampleTestGeneratorProvider : MsTestGeneratorProvider
        {
            public override void FinalizeTestClass(System.CodeDom.CodeNamespace codeNameSpace)
            {
                base.FinalizeTestClass(codeNameSpace);
                // change namespace
                codeNameSpace.Name = this.GetType().Name;

            }

            public override void SetTestVariant(CodeMemberMethod memberMethod, string title, string exampleName, IEnumerable<KeyValuePair<string, string>> arguments)
            {
                base.SetTestVariant(memberMethod, title, exampleName, arguments);

                Console.WriteLine("Titile:{0}", title);
                Console.WriteLine("exampleName:{0}", exampleName);             
            }
        }
    }

}
