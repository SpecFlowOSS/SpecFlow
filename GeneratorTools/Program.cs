using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using NConsoler;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Configuration;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Reporting;

namespace TechTalk.SpecFlow.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            Consolery.Run(typeof(Program), args);
            return;
        }

        [Action("Process all feature files")]
        public static void All(
            [Required] string projectFile
            )
        {
            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectFile);

            ProcessProject(specFlowProject);

//            Console.ReadLine();
        }

        [Action("Run all feature files")]
        public static void Run(
            [Required] string projectFile
            )
        {
            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectFile);

            RunProject(specFlowProject);

            Console.ReadLine();
        }

        private static void RunProject(SpecFlowProject specFlowProject)
        {
            Console.WriteLine("Processing project: " + specFlowProject);
//
//            AssemblyName name = AssemblyName.GetAssemblyName(
//                @"C:\Users\jba\Dev\Projects\SpecFlow\Samples\BowlingKata\Bowling.Specflow\bin\Debug\Bowling.dll");
//            AppDomain.CurrentDomain.Load(name);

            ITestRunner testRunner = new TestRunner();

            List<Assembly> bindingAssemblies = new List<Assembly>();

            string projectAssemblyPath = specFlowProject.ProjectFolder + @"\bin\Debug\" +  specFlowProject.AssemblyName + ".dll";
            Assembly projectAssembly = Assembly.LoadFrom(projectAssemblyPath);
            bindingAssemblies.Add(projectAssembly); 
//            string projectAssemblyDir = specFlowProject.ProjectFolder + @"\bin\Debug\";
//            string[] assemblies = Directory.GetFiles(projectAssemblyDir, "*.dll");
//            foreach (string assembly in assemblies)
//            {
//                Assembly projectAssembly2 = Assembly.LoadFrom(assembly);
//                AssemblyName name2 = AssemblyName.GetAssemblyName(assembly);
//                AppDomain.CurrentDomain.Load(name2);
////                bindingAssemblies.Add(projectAssembly2); 
//            }

            testRunner.InitializeTestRunner(bindingAssemblies.ToArray());


            foreach (var featureFile in specFlowProject.FeatureFiles)
            {
                SpecFlowLangParser parser = new SpecFlowLangParser(specFlowProject.GeneratorConfiguration.FeatureLanguage);
                Feature feature = parser.Parse(new StringReader(File.ReadAllText(featureFile.GetFullPath(specFlowProject))), featureFile.GetFullPath(specFlowProject));

                TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en"), "Score Calculation", "In order to know my performance\r\nAs a player\r\nI want the system to calculate my t" +
"otal score", ((string[])(null)));
                testRunner.OnFeatureStart(featureInfo);

                foreach (Scenario scenario in feature.Scenarios)
                {
                    TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Gutter game", ((string[])(null)));
                    testRunner.OnScenarioStart(scenarioInfo);

                    foreach (ScenarioStep scenarioStep in scenario.Steps)
                    {
                        TechTalk.SpecFlow.Table table1 = null;
                        if (scenarioStep.TableArg != null)
                        {
                            Console.WriteLine("Table: " + scenarioStep.TableArg);

                            table1 = new TechTalk.SpecFlow.Table(new string[] { "Pins" });
                            foreach (Row row in scenarioStep.TableArg.Body)
                            {
                                string[] cells = new string[row.Cells.Count()];
                                for (int i = 0; i < row.Cells.Count(); i++ )
                                {
                                    cells[i] = row.Cells[i].Value;
                                }
                                table1.AddRow(cells);
                            }

                        }

                        if (scenarioStep is Given)
                        {
                            Console.WriteLine("Given: " + scenarioStep.Text);
                            testRunner.Given(scenarioStep.Text);
                        }
                        else if (scenarioStep is When)
                        {
                            Console.WriteLine("When: " + scenarioStep.Text);
                            testRunner.When(scenarioStep.Text, null, table1);
                        }
                        else if (scenarioStep is Then)
                        {
                            Console.WriteLine("Then: " + scenarioStep.Text);
                            testRunner.Then(scenarioStep.Text);
                        }             
                        else if (scenarioStep is And)
                        {
                            Console.WriteLine("And: " + scenarioStep.Text);
                            testRunner.And(scenarioStep.Text);
                        }
                    }
                    testRunner.OnScenarioEnd();
                }
                testRunner.OnFeatureEnd();
            }
        }

        private static void ProcessProject(SpecFlowProject specFlowProject)
        {
            Console.WriteLine("Processing project: " + specFlowProject);

            foreach (var featureFile in specFlowProject.FeatureFiles)
            {
                string featureFileFullPath = featureFile.GetFullPath(specFlowProject);
 
                Console.WriteLine("Processing file: " + featureFileFullPath);

                SpecFlowGenerator generator = new SpecFlowGenerator(specFlowProject);
                CodeCompileUnit compileUnit = generator.GenerateTestFileCode(featureFile);

                string codeFileFullPath = featureFileFullPath + ".cs";
                using (var writer = new StreamWriter(codeFileFullPath, false, Encoding.UTF8))
                {
                    CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                    CodeGeneratorOptions options = new CodeGeneratorOptions();
                    options.BracingStyle = "C";
                    codeProvider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
                }
            }
        }
    }
}
