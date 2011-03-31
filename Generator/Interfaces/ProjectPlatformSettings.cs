using System;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    [Serializable]
    public class ProjectPlatformSettings
    {
        public string Language { get; set; }
        public Version LanguageVersion { get; set; }

        public string Platform { get; set; }
        public Version PlatformVersion { get; set; }

        public ProjectPlatformSettings()
        {
            Language = GenerationTargetLanguage.CSharp;
            LanguageVersion = new Version(3, 0);

            Platform = GenerationTargetPlatform.DotNet;
            PlatformVersion = new Version(3, 5);
        }
    }
}