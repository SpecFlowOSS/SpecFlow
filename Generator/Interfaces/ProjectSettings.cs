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

        public SpecFlowConfigurationHolder ConfigurationHolder { get; set; }

        public ProjectSettings()
        {
            ProjectPlatformSettings = new ProjectPlatformSettings();
            ConfigurationHolder = new SpecFlowConfigurationHolder();
        }
    }
}