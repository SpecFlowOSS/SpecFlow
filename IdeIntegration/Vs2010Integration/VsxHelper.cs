using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    internal class VsxHelper
    {
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

            DTE dte = (DTE)serviceProvider.GetService(typeof(DTE));

            ProjectItem prjItem = dte.Solution.FindProjectItem(ppzsFilename);

            return prjItem.ContainingProject;
        }

        public static IEnumerable<ProjectItem> GetAllProjectItem(EnvDTE.Project project)
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
                else if (project.ProjectItems != null)
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
    }
}
