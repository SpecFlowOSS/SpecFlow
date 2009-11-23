using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using NConsoler;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Configuration;
using TechTalk.SpecFlow.Reporting;

namespace TechTalk.SpecFlow.GeneratorTools
{
    class Program
    {
        static void Main(string[] args)
        {
            Consolery.Run(typeof(Program), args);
            return;
        }

        [Action("Generate tests from all feature files")]
        public static void Generate(
            [Required] string projectFile
            )
        {
            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectFile);

            ProcessProject(specFlowProject);
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