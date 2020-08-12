namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowDocumentLocation
    {
        public SpecFlowDocumentLocation(string sourceFilePath) : this(sourceFilePath, string.Empty)
        {}

        public SpecFlowDocumentLocation(string sourceFilePath, string featureFolderPath)
        {
            SourceFilePath = sourceFilePath;
            FeatureFolderPath = featureFolderPath ?? string.Empty;
        }

        /// <summary>
        /// Absolute path of the feature file in the file system
        /// </summary>
        public string SourceFilePath { get; private set; }

        /// <summary>
        /// Relative path within the project to the folder containing the feature, using '/' as path separator character
        /// </summary>
        /// <returns>A string representing the relative path or empty string if the feature file is in the root folder of the project</returns>
        /// <example>
        /// "[ProjectDir]\ARootFeature.feature" -> ""
        /// </example>
        /// <example>
        /// "[ProjectDir]\Features\SomeFeature.feature" -> "Features"
        /// </example>\
        /// <example>
        /// "[ProjectDir]\Features\Configuration\SomeConfigFeature.feature" -> "Features/Configuration"
        /// </example>
        public string FeatureFolderPath { get; private set; }
    }
}
