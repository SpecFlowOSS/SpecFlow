using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator
{
    public class SpecFlowGenerator
    {
        private readonly SpecFlowProject project;

        public SpecFlowGenerator(SpecFlowProject project)
        {
            this.project = project;
        }

        public void GenerateCSharpTestFile(SpecFlowFeatureFile featureFile, TextWriter outputWriter)
        {
            var codeProvider = new CSharpCodeProvider();
            GenerateTestFile(featureFile, codeProvider, outputWriter);
        }

        private void GenerateTestFile(SpecFlowFeatureFile featureFile, CodeDomProvider codeProvider, TextWriter outputWriter)
        {
            using(var reader = new StreamReader(featureFile.GetFullPath(project)))
            {
                GenerateTestFile(featureFile, codeProvider, reader, outputWriter);
            }
        }

        private class HackedWriter : TextWriter
        {
            TextWriter innerWriter;
            private bool trimSpaces = false;

            public HackedWriter(TextWriter innerWriter)
            {
                this.innerWriter = innerWriter;
            }

            public override void Write(char[] buffer, int index, int count)
            {
                Write(new string(buffer, index, count));
            }

            public override void Write(char value)
            {
                Write(value.ToString());
            }

            public override void Write(string value)
            {
                if (trimSpaces)
                {
                    value = value.TrimStart(' ', '\t');
                    if (value == string.Empty)
                        return;
                    trimSpaces = false;
                }

                innerWriter.Write(value);
            }

            public override Encoding Encoding
            {
                get { return innerWriter.Encoding; }
            }

            static public readonly Regex indentNextRe = new Regex(@"^[\s\/\']*#indentnext (?<ind>\d+)\s*$");

            public override void WriteLine(string text)
            {
                var match = indentNextRe.Match(text);
                if (match.Success)
                {
                    Write(new string(' ', int.Parse(match.Groups["ind"].Value)));
                    trimSpaces = true;
                    return;
                }

                base.WriteLine(text);
            }

            public override string ToString()
            {
                return innerWriter.ToString();
            }

            public override void Flush()
            {
                innerWriter.Flush();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    innerWriter.Dispose();
            }
        }


        public void GenerateTestFile(SpecFlowFeatureFile featureFile, CodeDomProvider codeProvider, TextReader inputReader, TextWriter outputWriter)
        {
            outputWriter = new HackedWriter(outputWriter);

            var codeNamespace = GenerateTestFileCode(featureFile, inputReader);
            var options = new CodeGeneratorOptions
                {
                    BracingStyle = "C"
                };

            AddSpecFlowHeader(codeProvider, outputWriter);
            codeProvider.GenerateCodeFromNamespace(codeNamespace, outputWriter, options);
            outputWriter.Flush();
        }

        public CodeNamespace GenerateTestFileCode(SpecFlowFeatureFile featureFile, TextReader inputReader)
        {
            string targetNamespace = GetTargetNamespace(featureFile);

            SpecFlowLangParser parser = new SpecFlowLangParser(project.GeneratorConfiguration.FeatureLanguage);
            Feature feature = parser.Parse(inputReader, featureFile.GetFullPath(project));

            IUnitTestGeneratorProvider generatorProvider = ConfigurationServices.CreateInstance<IUnitTestGeneratorProvider>(project.GeneratorConfiguration.GeneratorUnitTestProviderType);

            SpecFlowUnitTestConverter testConverter = new SpecFlowUnitTestConverter(generatorProvider, project.GeneratorConfiguration.AllowDebugGeneratedFiles);

            var codeNamespace = testConverter.GenerateUnitTestFixture(feature, null, targetNamespace);
            return codeNamespace;
        }

        private string GetTargetNamespace(SpecFlowFeatureFile featureFile)
        {
            if (!string.IsNullOrEmpty(featureFile.CustomNamespace))
                return featureFile.CustomNamespace;

            if (string.IsNullOrEmpty(project.DefaultNamespace))
                return null;

            string targetNamespace = project.DefaultNamespace;
            string projectFolder = project.ProjectFolder;
            string sourceFileFolder = Path.GetDirectoryName(featureFile.GetFullPath(project));
            if (sourceFileFolder.StartsWith(sourceFileFolder, StringComparison.InvariantCultureIgnoreCase))
            {
                string extraFolders = sourceFileFolder.Substring(projectFolder.Length);
                if (extraFolders.Length > 0)
                {
                    string[] parts = extraFolders.TrimStart('\\').Split('\\');
                    targetNamespace += "." + string.Join(".",
                                                 parts.Select(p => p.ToIdentifier()).ToArray());
                }
            }
            return targetNamespace;
        }

        private void AddSpecFlowHeader(CodeDomProvider codeProvider, TextWriter outputWriter)
        {
            var specFlowHeaderTemplate = @"------------------------------------------------------------------------------
 <auto-generated>
     This code was generated by SpecFlow (http://www.specflow.org/).
     SpecFlow Version:{0}
     Runtime Version:{1}

     Changes to this file may cause incorrect behavior and will be lost if
     the code is regenerated.
 </auto-generated>
------------------------------------------------------------------------------";

            var headerReader = new StringReader(string.Format(specFlowHeaderTemplate, 
                GetCurrentSpecFlowVersion(),
                Environment.Version
                ));

            string line;
            while ((line = headerReader.ReadLine()) != null)
            {
                codeProvider.GenerateCodeFromStatement(new CodeCommentStatement(line), outputWriter, null);
            }
        }

        public Version GetCurrentSpecFlowVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        static private readonly Regex versionRe = new Regex(@"SpecFlow Version:\s*(?<ver>\d+\.\d+\.\d+\.\d+)");

        public Version GetGeneratedFileSpecFlowVersion(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return null;

                using (var reader = new StreamReader(filePath))
                {
                    return GetGeneratedFileSpecFlowVersion(reader);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        private Version GetGeneratedFileSpecFlowVersion(StreamReader reader)
        {
            try
            {
                const int maxLinesToScan = 10;

                int lineNo = 0;
                string line;
                while ((line = reader.ReadLine()) != null && lineNo < maxLinesToScan)
                {
                    var match = versionRe.Match(line);
                    if (match.Success)
                        return new Version(match.Groups["ver"].Value);

                    lineNo++;
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }
    }
}