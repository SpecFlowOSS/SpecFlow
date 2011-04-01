using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using Microsoft.CSharp;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Utils;

namespace ParserTests
{
    /// <summary>
    /// A test for testing cusomterized test generator provider
    /// </summary>
    [TestFixture]
    public class SimpleTestGeneratorProviderTest
    {
        /// <summary>
        /// Generates the scenario example tests.
        /// </summary>
        [Test]
        public void GenerateScenarioExampleTests()
        {
            SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo("en-US"));
            foreach (var testFile in TestFileHelper.GetTestFiles())
            {
                using (var reader = new StreamReader(testFile))
                {
                    Feature feature = parser.Parse(reader, null);                    
                    Assert.IsNotNull(feature);

                    Console.WriteLine("Testing {0}", Path.GetFileName(testFile));

                    var codeDomHelper = new CodeDomHelper(CodeDomProviderLanguage.CSharp);
                    var sampleTestGeneratorProvider = new SimpleTestGeneratorProvider();
                    var converter = new SpecFlowUnitTestConverter(sampleTestGeneratorProvider, codeDomHelper, true, true);
                    CodeNamespace code = converter.GenerateUnitTestFixture(feature, "TestClassName", "Target.Namespace");

                    Assert.IsNotNull(code);
                  
                    // make sure name space is changed
                    Assert.AreEqual(code.Name, SimpleTestGeneratorProvider.DefaultNameSpace);

                    // make sure all method titles are changed correctly
                    List<string> methodTitles = new List<string>();
                    for (int i = 0; i < code.Types[0].Members.Count; i++)
                    {
                        methodTitles.Add(code.Types[0].Members[i].Name);
                    }

                    foreach (var title in sampleTestGeneratorProvider.newTitles)
                    {
                        Assert.IsTrue(methodTitles.Contains(title));
                    }
                }
            }
        }

        /// <summary>
        /// This class will change the default name space of a geneated test code, and the method name for tests corresponds to a scenario example
        /// </summary>
        class SimpleTestGeneratorProvider : MsTestGeneratorProvider
        {
            public static string DefaultNameSpace
            {
                get
                {
                    return "SampleTestGeneratorProvider";
                }
            }

            public override void FinalizeTestClass(System.CodeDom.CodeNamespace codeNameSpace)
            {
                base.FinalizeTestClass(codeNameSpace);
                // change namespace 
                codeNameSpace.Name = DefaultNameSpace;

            }

            public override void SetTestVariant(CodeMemberMethod memberMethod, string title, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments)
            {
                base.SetTestVariant(memberMethod, title, exampleSetName, variantName, arguments);

                // change memberMethodName
                memberMethod.Name = GetMethodName(title, exampleSetName, arguments);
                newTitles.Add(memberMethod.Name);
            }

            public List<string> newTitles = new List<string>();

            /// <summary>
            /// Gets the name of the method by concat the name and value of arugment together
            /// </summary>
            /// <param name="title">The title.</param>
            /// <param name="exampleSetTitle">The example set title.</param>
            /// <param name="arguments">The arguments.</param>
            /// <returns></returns>
            private static string GetMethodName(string title, string exampleSetTitle, IEnumerable<KeyValuePair<string, string>> arguments)
            {
                string testMethodName = string.IsNullOrEmpty(exampleSetTitle)
               ? string.Format("{0}", title)
               : string.Format("{0}_{1}", title, exampleSetTitle);

                testMethodName += string.Concat(arguments.Select(kp => "_" + kp.Key + "_" + kp.Value).ToArray());
                IEnumerable<char> chars = from ch in testMethodName
                                          where (char.IsLetter(ch) || char.IsNumber(ch) || ch == '_') && ch != ' '
                                          select ch;
                testMethodName = new string(chars.ToArray());
                return testMethodName;
            }
        }
    }
}
