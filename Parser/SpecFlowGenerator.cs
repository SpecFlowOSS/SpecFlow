using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Parser.Configuration;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowGenerator
    {
        private readonly SpecFlowProject project;

        public SpecFlowGenerator(SpecFlowProject project)
        {
            this.project = project;
        }

        public CodeCompileUnit GenerateTestFileCode(string featureFileName, string content)
        {
            SpecFlowFeatureFile featureFile = GetFeatureFile(featureFileName);
            string targetNamespace = GetTargetNamespace(featureFile);

            SpecFlowLangParser parser = new SpecFlowLangParser();
            Feature feature = parser.Parse(new StringReader(content), featureFile.GetFullPath(project));

            IUnitTestConverter converter = CreateInstance<IUnitTestConverter>(project.GeneratorConfiguration.GeneratorUnitTestProviderType);

            SpecFlowUnitTestConverter testConverter = new SpecFlowUnitTestConverter(converter);
            CodeCompileUnit codeCompileUnit = testConverter.GenerateUnitTestFixture(feature, null, targetNamespace);

            return codeCompileUnit;
        }

        private static TInterface CreateInstance<TInterface>(Type type)
        {
            //TODO: better error handling
            return (TInterface)Activator.CreateInstance(type);
        }

        private SpecFlowFeatureFile GetFeatureFile(string featureFileName)
        {
            featureFileName = Path.GetFullPath(Path.Combine(project.ProjectFolder, featureFileName));
            var result = project.FeatureFiles.Find(ff => ff.GetFullPath(project).Equals(featureFileName, StringComparison.InvariantCultureIgnoreCase));
            if (result == null)
            {
                result = new SpecFlowFeatureFile(featureFileName); //TODO: make it project relative
            }
            return result;
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
                //targetNamespace += extraFolders.Replace("\\", ".");
            }
            return targetNamespace;
        }
    }
}
