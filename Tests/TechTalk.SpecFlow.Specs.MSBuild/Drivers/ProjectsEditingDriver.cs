using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.MSBuild.Drivers
{
    public class ProjectsEditingDriver
    {
        private readonly SolutionDriver _solutionDriver;

        private readonly TestProjectFolders _testProjectFolders;

        private static readonly Regex FeatureRegex = new Regex(@"^Feature:\s*([A-Za-z0-9]+.*$)");

        public ProjectsEditingDriver(SolutionDriver solutionDriver, TestProjectFolders testProjectFolders)
        {
            _solutionDriver = solutionDriver;
            _testProjectFolders = testProjectFolders;
        }

        public void RemoveFeature(string featureName)
        {
            // Search through the feature files in the directory to find one with the expected title.
            foreach (var file in Directory.EnumerateFiles(_testProjectFolders.ProjectFolder, "*.feature"))
            {
                var lines = File.ReadAllLines(file);

                foreach (var line in lines)
                {
                    var match = FeatureRegex.Match(line);

                    if (match.Success && match.Groups[1].Value == featureName)
                    {
                        RemoveFile(file);
                        return;
                    }
                }
            }

            throw new InvalidOperationException($"Couldn't find a feature file for feature \"{featureName}\"");
        }

        public void RemoveFile(string fileName)
        {
            var filePath = Path.IsPathRooted(fileName) ? fileName : Path.Combine(_testProjectFolders.ProjectFolder, fileName);

            // Remove the written feature file.            
            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException("Feature file does not exist.");
            }

            File.Delete(filePath);

            // If the project is in the old format, remove the entry in the project file format.
            if (_solutionDriver.DefaultProject.Format == TestProjectGenerator.Data.ProjectFormat.Old)
            {
                RemoveFileFromProject(fileName);
            }
        }

        private void RemoveFileFromProject(string fileName)
        {
            var projectFile = Path.Combine(_testProjectFolders.ProjectFolder, _solutionDriver.DefaultProject.ProjectName + ".csproj");

            XDocument project;
            using (var fs = File.OpenRead(projectFile))
            {
                project = XDocument.Load(fs);
            }

            project
                .Descendants()
                .Where(element => element.Attributes(XName.Get("Include")).Any(attr => attr.Value == fileName))
                .Remove();

            var writerSettings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            };

            using (var writer = XmlWriter.Create(File.Open(projectFile, FileMode.Truncate), writerSettings))
            {
                project.WriteTo(writer);
                writer.Flush();
            }
        }
    }
}
