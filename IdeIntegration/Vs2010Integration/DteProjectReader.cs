using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    internal class DteProjectReader
    {
        private static bool IsProjectSupported(Project project)
        {
            return
                project.FullName.EndsWith(".csproj") ||
                project.FullName.EndsWith(".vbproj");
            //                kind.Equals(vsContextGuids.vsContextGuidVCSProject, StringComparison.InvariantCultureIgnoreCase) ||
            //                kind.Equals(vsContextGuids.vsContextGuidVBProject, StringComparison.InvariantCultureIgnoreCase);
        }

        public static SpecFlowProject LoadSpecFlowProjectFromDteProject(Project project)
        {
            if (project == null || !IsProjectSupported(project))
                return null;

            try
            {
                return LoadSpecFlowProjectFromDteProjectInternal(project);
            }
            catch
            {
                return null;
            }
        }

        public static SpecFlowProjectConfiguration LoadSpecFlowConfigurationFromDteProject(Project project)
        {
            if (project == null || !IsProjectSupported(project))
                return null;

            try
            {
                return LoadSpecFlowConfigurationFromDteProjectInternal(project);
            }
            catch
            {
                return null;
            }
        }

        private static SpecFlowProject LoadSpecFlowProjectFromDteProjectInternal(Project project)
        {
            string projectFolder = Path.GetDirectoryName(project.FullName);

            SpecFlowProject specFlowProject = new SpecFlowProject();
            specFlowProject.ProjectFolder = projectFolder;
            specFlowProject.ProjectName = Path.GetFileNameWithoutExtension(project.FullName);
            specFlowProject.AssemblyName = project.Properties.Item("AssemblyName").Value as string;
            specFlowProject.DefaultNamespace = project.Properties.Item("DefaultNamespace").Value as string;

            foreach (ProjectItem projectItem in VsxHelper.GetAllProjectItem(project).Where(IsPhysicalFile))
            {
                var fileName = GetRelativePath(GetFileName(projectItem), projectFolder);

                var extension = Path.GetExtension(fileName);
                if (extension.Equals(".feature", StringComparison.InvariantCultureIgnoreCase))
                {
                    var featureFile = new SpecFlowFeatureFile(fileName);
                    var ns = projectItem.Properties.Item("CustomToolNamespace").Value as string;
                    if (!String.IsNullOrEmpty(ns))
                        featureFile.CustomNamespace = ns;
                    specFlowProject.FeatureFiles.Add(featureFile);
                }
            }

            specFlowProject.Configuration = LoadSpecFlowConfigurationFromDteProjectInternal(project);

            return specFlowProject;
        }

        private static SpecFlowProjectConfiguration LoadSpecFlowConfigurationFromDteProjectInternal(Project project)
        {
            string projectFolder = Path.GetDirectoryName(project.FullName);
            SpecFlowProjectConfiguration configuration = new SpecFlowProjectConfiguration();
            foreach (ProjectItem projectItem in VsxHelper.GetAllProjectItem(project).Where(IsPhysicalFile))
            {
                var fileName = GetRelativePath(GetFileName(projectItem), projectFolder);
                if (Path.GetFileName(fileName).Equals("app.config", StringComparison.InvariantCultureIgnoreCase))
                {
                    string configFileContent = GetFileContent(projectItem);
                    GeneratorConfigurationReader.UpdateConfigFromFileContent(configuration.GeneratorConfiguration, configFileContent);
                    RuntimeConfigurationReader.UpdateConfigFromFileContent(configuration.RuntimeConfiguration, configFileContent);
                }
            }
            return configuration;
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