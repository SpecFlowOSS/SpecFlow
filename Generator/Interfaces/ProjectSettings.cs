using System;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    [Serializable]
    public class ProjectSettings
    {
        /// <summary>
        /// The name of the project. Mandatory.
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// The assembly name of the compiled project (without .dll extension). Mandatory.
        /// </summary>
        public string AssemblyName { get; set; }
        /// <summary>
        /// The root folder of the project. Mandatory.
        /// </summary>
        public string ProjectFolder { get; set; }
        /// <summary>
        /// The default namespace of the project. Optional.
        /// </summary>
        public string DefaultNamespace { get; set; }
        /// <summary>
        /// The plaform settings of the project. Mandatory.
        /// </summary>
        public ProjectPlatformSettings ProjectPlatformSettings { get; set; }

        /// <summary>
        /// The reference of the unparsed SpecFlow configuration of the project. Mandatory.
        /// </summary>
        public SpecFlowConfigurationHolder ConfigurationHolder { get; set; }

        public ProjectSettings()
        {
            ProjectPlatformSettings = new ProjectPlatformSettings();
            ConfigurationHolder = new SpecFlowConfigurationHolder();
        }
    }
}