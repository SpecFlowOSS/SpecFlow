using System;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    [Serializable]
    public class GenerationSettings
    {
        public string TargetLanguage { get; set; }
        public Version TargetLanguageVersion { get; set; }
        
        public string TargetPlatform { get; set; }
        public Version TargetPlatformVersion { get; set; }

        public string ProjectDefaultNamespace { get; set; }

        public bool CheckUpToDate { get; set; }

        public GenerationSettings()
        {
            TargetLanguage = GenerationTargetLanguage.CSharp;
            TargetLanguageVersion = new Version(3, 0);

            TargetPlatform = GenerationTargetPlatform.DotNet;
            TargetPlatformVersion = new Version(3, 5);

            ProjectDefaultNamespace = "SpecFlow.GeneratedTests";

            CheckUpToDate = false;
        }
    }
}