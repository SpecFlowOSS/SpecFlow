using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpecFlow.TestProjectGenerator
{
    public class ProjectCompilerHelper
    {
        protected readonly Folders _folders;

        protected ProjectCompilerHelper(Folders folders)
        {
            _folders = folders;
        }

        public string SaveFileFromResourceTemplate(string compilationFolder, string templateName, string outputFileName, Dictionary<string, string> replacements = null)
        {
            return SaveFileFromResourceTemplate(compilationFolder, templateName, outputFileName, Encoding.UTF8, replacements);
        }

        public string SaveFileFromResourceTemplate(string compilationFolder, string templateName, string outputFileName, Encoding encoding, Dictionary<string, string> replacements = null)
        {
            return SaveFileFromResourceTemplate(typeof(ProjectCompilerHelper), compilationFolder, templateName, outputFileName, encoding, replacements);
        }

        public string SaveFileFromResourceTemplate(Type type, string compilationFolder, string templateName, string outputFileName, Encoding encoding, Dictionary<string, string> replacements = null)
        {
            var assembly = Assembly.GetAssembly(type);
            Debug.Assert(assembly != null);

            string resourceNameToFind = "TestProjectTemplates." + templateName;
            var resourceName = assembly.GetManifestResourceNames().Where(i => i.EndsWith(resourceNameToFind)).SingleOrDefault();
            Debug.Assert(resourceName != null);

            var projectTemplateStream = assembly.GetManifestResourceStream(resourceName);
            Debug.Assert(projectTemplateStream != null);

            string template = new StreamReader(projectTemplateStream).ReadToEnd();

            return SaveFileFromTemplate(compilationFolder, template, outputFileName, encoding, replacements);
        }

        public string SaveFileFromTemplate(string compilationFolder, string templateContent, string outputFileName, Dictionary<string, string> replacements = null)
        {
            return SaveFileFromTemplate(compilationFolder, templateContent, outputFileName, Encoding.UTF8, replacements);
        }

        public string SaveFileFromTemplate(string compilationFolder, string templateContent, string outputFileName, Encoding encoding, Dictionary<string, string> replacements = null)
        {
            string outputPath = Path.Combine(compilationFolder, outputFileName);
            string fileContent = templateContent;

            if (replacements != null)
            {
                fileContent = replacements.Aggregate(fileContent, (current, replacement) => current.Replace("{" + replacement.Key + "}", replacement.Value));
            }

            fileContent = ReplacePlaceholders(fileContent);

            File.WriteAllText(outputPath, fileContent, encoding);

            return outputPath;
        }

        protected virtual string ReplacePlaceholders(string fileContent)
        {
            return fileContent
                .Replace("{SpecFlowRoot}", _folders.SpecFlow)
                .Replace("{SupportedFramework}", CurrentVersionDriver.SpecFlowMajor >= 2 ? "NET45" : "NET35")
                .Replace("{SpecFlowDev}", Path.Combine(_folders.TestFolder, "SpecFlowDevelopment"));
        }
    }
}