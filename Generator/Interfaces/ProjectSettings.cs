using System;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    [Serializable]
    public class ProjectSettings
    {
        public string ProjectFolder { get; set; }
        public string DefaultNamespace { get; set; }
        public ProjectPlatformSettings ProjectPlatformSettings { get; set; }

        public string ProjectName { get; set; }
        public string AssemblyName { get; set; }

        public ProjectSettings(string projectFolder, string defaultNamespace, ProjectPlatformSettings projectPlatformSettings)
        {
            if (projectFolder == null) throw new ArgumentNullException("projectFolder");
            if (defaultNamespace == null) throw new ArgumentNullException("defaultNamespace");

            ProjectFolder = projectFolder;
            DefaultNamespace = defaultNamespace;
            ProjectPlatformSettings = projectPlatformSettings ?? new ProjectPlatformSettings();
        }
    }
}