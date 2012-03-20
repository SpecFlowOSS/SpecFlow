using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Vs2008Integration
{
    internal static class VsxHelper
    {
        public static IEnumerable<ProjectItem> GetAllProjectItem(Project project)
        {
            return GetAllProjectItem(project.ProjectItems);
        }

        private static IEnumerable<ProjectItem> GetAllProjectItem(ProjectItems projectItems)
        {
            Queue<ProjectItem> items = new Queue<ProjectItem>();
            foreach (ProjectItem item in projectItems)
                items.Enqueue(item);
            while (items.Count != 0)
            {
                ProjectItem item = items.Dequeue();
                yield return item;
                if (item.ProjectItems != null)
                    foreach (ProjectItem subitem in item.ProjectItems)
                        items.Enqueue(subitem);
            }
        }

        public static IEnumerable<ProjectItem> GetAllPhysicalFileProjectItem(Project project)
        {
            return GetAllProjectItem(project).Where(IsPhysicalFile);
        }

        public static ProjectItem FindProjectItemByProjectRelativePath(Project project, string filePath)
        {
            return GetAllPhysicalFileProjectItem(project).FirstOrDefault(
                    pi => filePath.Equals(GetProjectRelativePath(pi), StringComparison.InvariantCultureIgnoreCase));
        }

        public static string GetFileName(ProjectItem projectItem)
        {
            if (!IsPhysicalFile(projectItem))
                return null;

            return projectItem.FileNames[1];
        }

        public static string GetProjectRelativePath(ProjectItem projectItem)
        {
            string fileName = GetFileName(projectItem);
            if (fileName == null)
                return null;

            string projectFolder = GetProjectFolder(projectItem.ContainingProject);
            return FileSystemHelper.GetRelativePath(fileName, projectFolder);
        }

        public static string GetProjectFolder(Project project)
        {
            return Path.GetDirectoryName(project.FullName);
        }

        public static bool IsPhysicalFile(ProjectItem projectItem)
        {
            return String.Equals(projectItem.Kind, VSConstants.GUID_ItemType_PhysicalFile.ToString("B"), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
