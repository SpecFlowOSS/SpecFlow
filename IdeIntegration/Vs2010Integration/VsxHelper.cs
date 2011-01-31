using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Vs2010Integration.Utils;
using Constants = EnvDTE.Constants;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    internal static class VsxHelper
    {
        public static DTE GetDte(SVsServiceProvider serviceProvider)
        {
            return (DTE)serviceProvider.GetService(typeof(DTE));
        }

        public static Project GetCurrentProject(ITextBuffer buffer, IVsEditorAdaptersFactoryService adaptersFactory, SVsServiceProvider serviceProvider)
        {
            IPersistFileFormat persistFileFormat = adaptersFactory.GetBufferAdapter(buffer) as IPersistFileFormat;
            if (persistFileFormat == null)
                return null;

            string ppzsFilename;
            uint iii;
            persistFileFormat.GetCurFile(out ppzsFilename, out iii);

            if (String.IsNullOrWhiteSpace(ppzsFilename))
                return null;

            DTE dte = GetDte(serviceProvider);

            ProjectItem prjItem = dte.Solution.FindProjectItem(ppzsFilename);

            return prjItem.ContainingProject;
        }

        public static IEnumerable<ProjectItem> GetAllProjectItem(Project project)
        {
            Queue<ProjectItem> items = new Queue<ProjectItem>();
            foreach (ProjectItem item in project.ProjectItems)
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

        /// <summary>
        /// Retrieves the first project in the solution that matches the specified criteria.
        /// </summary>
        /// <param name="vs">The VS instance.</param>
        /// <param name="match">The predicate condition.</param>
        /// <returns>The project found or <see langword="null"/>.</returns>
        public static Project FindProject(_DTE vs, Predicate<Project> match)
        {
            if (vs == null) throw new ArgumentNullException("vs");
            if (match == null) throw new ArgumentNullException("match");

            foreach (Project project in vs.Solution.Projects)
            {
                if (match(project))
                {
                    return project;
                }
                
                if (project.ProjectItems != null)
                {
                    Project child = FindProjectInternal(project.ProjectItems, match);
                    if (child != null)
                    {
                        return child;
                    }
                }
            }

            return null;
        }

        private static Project FindProjectInternal(ProjectItems items, Predicate<Project> match)
        {
            foreach (ProjectItem item in items)
            {
                Project project = item.Object as Project;

                if (project == null && item.SubProject != null)
                {
                    project = item.SubProject as Project;
                }

                if (project != null && match(project))
                {
                    return project;
                }
                else if (item.ProjectItems != null)
                {
                    Project child = FindProjectInternal(item.ProjectItems, match);
                    if (child != null)
                    {
                        return child;
                    }
                }
                else if (project != null && project.ProjectItems != null)
                {
                    Project child = FindProjectInternal(project.ProjectItems, match);
                    if (child != null)
                    {
                        return child;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a project in the solution, given its output assembly name.
        /// </summary>
        /// <returns>A <see cref="Project"/> reference or <see langword="null" /> if 
        /// it doesn't exist. Project can be C# or VB.</returns>
        public static Project FindProjectByAssemblyName(_DTE vs, string name)
        {
            return FindProject(vs, delegate(Project project)
            {
                Property prop = project.Properties.Cast<Property>().FirstOrDefault(p => p.Name == "AssemblyName");
                return prop != null && prop.Value != null &&
                    prop.Value.ToString() == name;
            });
        }

        /// <summary>
        /// Finds a project in the solution, given its name.
        /// </summary>
        /// <returns>A <see cref="Project"/> reference or <see langword="null" /> if 
        /// it doesn't exist. Project can be C# or VB.</returns>
        public static Project FindProjectByName(_DTE vs, string name)
        {
            return FindProject(vs, delegate(Project project)
            {
                return project.Name == name;
            });
        }

        public static T GetOption<T>(DTE dte, string categoryName, string pageName, string optionName, T defaultValue = default(T))
        {
            Properties properties = dte.Properties[categoryName, pageName];
            if (properties == null)
                return defaultValue;

            Property property = properties.Item(optionName);
            if (property == null || !(property.Value is T))
                return defaultValue;

            return (T)property.Value;
        }

        public static bool IsPhysicalFile(ProjectItem projectItem)
        {
            return String.Equals(projectItem.Kind, VSConstants.GUID_ItemType_PhysicalFile.ToString("B"), StringComparison.InvariantCultureIgnoreCase);
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

            string projectFolder = Path.GetDirectoryName(projectItem.ContainingProject.FullName);
            return FileSystemHelper.GetRelativePath(fileName, projectFolder);
        }

        static public string GetFileContent(ProjectItem projectItem)
        {
            if (projectItem.IsOpen[Constants.vsViewKindAny])
            {
                TextDocument textDoc = (TextDocument)projectItem.Document.Object("TextDocument");
                EditPoint start = textDoc.StartPoint.CreateEditPoint();
                return start.GetText(textDoc.EndPoint);
            }
            
            using (TextReader file = new StreamReader(GetFileName(projectItem)))
            {
                return file.ReadToEnd();
            }
        }
    }
}
