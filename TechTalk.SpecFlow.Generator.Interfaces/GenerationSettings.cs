using System;

namespace TechTalk.SpecFlow.Generator
{
    [Serializable]
    public class GenerationSettings
    {
        public string TargetLanguage { get; private set; }
        public Version TargetLanguageVersion { get; private set; }
        
        public string TargetPlatform { get; private set; }
        public Version TargetPlatformVersion { get; private set; }

        public string ProjectDefaultNamespace { get; private set; }

        public GenerationSettings(string targetLanguage, string targetLanguageVersion, string targetPlatform, string targetPlatformVersion, string projectDefaultNamespace)
            : this(targetLanguage, new Version(targetLanguageVersion), targetPlatform, new Version(targetPlatformVersion), projectDefaultNamespace)
        {
            
        }

        public GenerationSettings(string targetLanguage, Version targetLanguageVersion, string targetPlatform, Version targetPlatformVersion, string projectDefaultNamespace)
        {
            TargetLanguage = targetLanguage;
            TargetLanguageVersion = targetLanguageVersion;
            TargetPlatform = targetPlatform;
            TargetPlatformVersion = targetPlatformVersion;
            ProjectDefaultNamespace = projectDefaultNamespace;
        }
    }
}