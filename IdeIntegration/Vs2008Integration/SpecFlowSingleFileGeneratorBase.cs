using System;
using System.CodeDom.Compiler;
using EnvDTE;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.VsIntegration.Common
{
    [System.Runtime.InteropServices.ComVisible(true)]
    [Guid("AC7359F9-6206-47A3-935C-EF191D73B9ED")]
    public abstract partial class SpecFlowSingleFileGeneratorBase : BaseCodeGeneratorWithSite
    {
        protected override string GetDefaultExtension()
        {
            CodeDomProvider provider = GetCodeProvider();

            return ".feature." + provider.FileExtension;
        }

        protected override string GenerateCode(string inputFileContent)
        {
            CodeDomProvider provider = GetCodeProvider();

            SpecFlowProject specFlowProject = DteProjectReader.LoadSpecFlowProjectFromDteProject(CurrentProject);
            SpecFlowGenerator generator = new SpecFlowGenerator(specFlowProject);

            using (var writer = new StringWriter(new StringBuilder()))
            {
                generator.GenerateTestFile(
                    specFlowProject.GetOrCreateFeatureFile(CodeFilePath), 
                    provider, new StringReader(inputFileContent), writer);
                return writer.ToString();
            }
        }

        protected override string GenerateError(Microsoft.VisualStudio.Shell.Interop.IVsGeneratorProgress pGenerateProgress, Exception ex)
        {
            if (ex is SpecFlowParserException)
            {
                SpecFlowParserException sfpex = (SpecFlowParserException) ex;
                if (sfpex.ErrorDetails == null || sfpex.ErrorDetails.Count == 0)
                    return base.GenerateError(pGenerateProgress, ex);

                foreach (var errorDetail in sfpex.ErrorDetails)
                    pGenerateProgress.GeneratorError(0, 4, errorDetail.Message, 
                        (uint)errorDetail.ForcedLine - 1,
                        (uint)errorDetail.ForcedColumn - 1);

                return GetMessage(ex);
            }

            return base.GenerateError(pGenerateProgress, ex);
        }
    }

    internal class DteProjectReader
    {
        public static List<ProjectItem> GetAllProjectItem(EnvDTE.Project project)
        {
            List<ProjectItem> retval = new List<ProjectItem>();
            Queue<ProjectItem> items = new Queue<ProjectItem>();
            foreach (ProjectItem item in project.ProjectItems)
                items.Enqueue(item);
            while (items.Count != 0)
            {
                ProjectItem item = items.Dequeue();
                retval.Add(item);
                if (item.ProjectItems != null)
                    foreach (ProjectItem subitem in item.ProjectItems)
                        items.Enqueue(subitem);
            }
            return retval;
        }


        public static SpecFlowProject LoadSpecFlowProjectFromDteProject(Project project)
        {
            string projectFolder = Path.GetDirectoryName(project.FullName);

            SpecFlowProject specFlowProject = new SpecFlowProject();
            specFlowProject.ProjectSettings.ProjectFolder = projectFolder;
            specFlowProject.ProjectSettings.ProjectName = Path.GetFileNameWithoutExtension(project.FullName);
            specFlowProject.ProjectSettings.AssemblyName = project.Properties.Item("AssemblyName").Value as string;
            specFlowProject.ProjectSettings.DefaultNamespace = project.Properties.Item("DefaultNamespace").Value as string;

            foreach (ProjectItem projectItem in GetAllProjectItem(project).Where(IsPhysicalFile))
            {
                var fileName = GetRelativePath(GetFileName(projectItem), projectFolder);

                var extension = Path.GetExtension(fileName);
                if (extension.Equals(".feature", StringComparison.InvariantCultureIgnoreCase))
                {
                    var featureFile = new FeatureFileInput(fileName);
                    var ns = projectItem.Properties.Item("CustomToolNamespace").Value as string;
                    if (!String.IsNullOrEmpty(ns))
                        featureFile.CustomNamespace = ns;
                    specFlowProject.FeatureFiles.Add(featureFile);
                }

                if (Path.GetFileName(fileName).Equals("app.config", StringComparison.InvariantCultureIgnoreCase))
                {
                    GeneratorConfigurationReader.UpdateConfigFromFileContent(specFlowProject.Configuration.GeneratorConfiguration, GetFileContent(projectItem));
                }

            }
            return specFlowProject;
        }

        private static bool IsPhysicalFile(ProjectItem projectItem)
        {
            return string.Equals(projectItem.Kind, VSConstants.GUID_ItemType_PhysicalFile.ToString("B"), StringComparison.InvariantCultureIgnoreCase);
        }

        static public string GetFileContent(ProjectItem projectItem)
        {
            if (projectItem.get_IsOpen(EnvDTE.Constants.vsViewKindAny))
            {
                TextDocument textDoc = (TextDocument)projectItem.Document.Object("TextDocument");
                EditPoint start = textDoc.StartPoint.CreateEditPoint();
                return start.GetText(textDoc.EndPoint);
            }
            else
            {
                using (TextReader file = new StreamReader(GetFileName(projectItem)))
                {
                    return file.ReadToEnd();
                }
            }
        }

        private static string GetFileName(ProjectItem projectItem)
        {
            return projectItem.get_FileNames(1);
        }

        public static string GetRelativePath(string path, string basePath)
        {
            path = Path.GetFullPath(path);
            basePath = Path.GetFullPath(basePath);
            if (string.Equals(path, basePath, StringComparison.OrdinalIgnoreCase))
                return "."; // the "this folder"

            if (path.StartsWith(basePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                return path.Substring(basePath.Length + 1);

            //handle different drives
            string pathRoot = Path.GetPathRoot(path);
            if (!string.Equals(pathRoot, Path.GetPathRoot(basePath), StringComparison.OrdinalIgnoreCase))
                return path;

            //handle ".." pathes
            string[] pathParts = path.Substring(pathRoot.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string[] basePathParts = basePath.Substring(pathRoot.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            int commonFolderCount = 0;
            while (commonFolderCount < pathParts.Length && commonFolderCount < basePathParts.Length &&
                string.Equals(pathParts[commonFolderCount], basePathParts[commonFolderCount], StringComparison.OrdinalIgnoreCase))
                commonFolderCount++;

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < basePathParts.Length - commonFolderCount; i++)
            {
                result.Append("..");
                result.Append(Path.DirectorySeparatorChar);
            }

            if (pathParts.Length - commonFolderCount == 0)
                return result.ToString().TrimEnd(Path.DirectorySeparatorChar);

            result.Append(string.Join(Path.DirectorySeparatorChar.ToString(), pathParts, commonFolderCount, pathParts.Length - commonFolderCount));
            return result.ToString();
        }

        private static string DumpProperties(ProjectItem projectItem)
        {
            StringBuilder result = new StringBuilder();
            foreach (Property property in projectItem.Properties)
            {
                result.AppendLine(property.Name + "=" + property.Value);
            }
            return result.ToString();
        }
    }
}
