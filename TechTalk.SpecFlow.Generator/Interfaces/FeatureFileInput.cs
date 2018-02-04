using System;
using System.IO;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    /// IMPORTANT
    /// This class is used for interop with the Visual Studio Extension
    /// DO NOT REMOVE OR RENAME FIELDS!
    /// This breaks binary serialization accross appdomains
    /// 
    /// 
    /// <summary>
    /// Represents the information related to a feature file as an input of the generation
    /// </summary>
    [Serializable]
    public class FeatureFileInput
    {
        /// <summary>
        /// The project relative path of the feature file. Must be sepecified.
        /// </summary>
        public string ProjectRelativePath { get; private set; }
        /// <summary>
        /// The content of the feature file. Optional. If not specified, the content is read from <see cref="ProjectRelativePath"/>.
        /// </summary>
        public string FeatureFileContent { get; set; }
        /// <summary>
        /// A custom namespace for the generated test class. Optional.
        /// </summary>
        public string CustomNamespace { get; set; }

        /// <summary>
        /// The project relative path of the generated test class file. Optional, used for up-to-date checking. 
        /// </summary>
        public string GeneratedTestProjectRelativePath { get; set; }
        /// <summary>
        /// The content of the existing test class file. Optional, used for up-to-date checking. 
        /// </summary>
        public string GeneratedTestFileContent { get; set; }

        public FeatureFileInput(string projectRelativePath)
        {
            if (projectRelativePath == null) throw new ArgumentNullException("projectRelativePath");
            if (string.IsNullOrEmpty(Path.GetFileName(projectRelativePath))) 
                throw new ArgumentException("The feature file path must denote a file and not a directory", "projectRelativePath");

            ProjectRelativePath = projectRelativePath;
        }
    }
}