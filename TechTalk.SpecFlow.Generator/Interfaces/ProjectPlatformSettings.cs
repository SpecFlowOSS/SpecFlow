using System;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    /// IMPORTANT
    /// This class is used for interop with the Visual Studio Extension
    /// DO NOT REMOVE OR RENAME FIELDS!
    /// This breaks binary serialization across appdomains
    [Serializable]
    public class ProjectPlatformSettings
    {
        /// <summary>
        /// Specifies the programming language of the project. Optional, defaults to C# 3.0.
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// The version of the <see cref="Language"/>. Optional, defaults to C# 3.0.
        /// </summary>
        [Obsolete("Not used anymore, will be removed with SpecFlow future versions")]
        public Version LanguageVersion { get; set; }

        /// <summary>
        /// Specifies the target platform of the project. Optional, defaults to .NET 3.5.
        /// </summary>
        [Obsolete("Not used anymore, will be removed with SpecFlow future versions")]
        public string Platform { get; set; }
        /// <summary>
        /// The version of the <see cref="Platform"/>. Optional, defaults to .NET 3.5.
        /// </summary>
        [Obsolete("Not used anymore, will be removed with SpecFlow future versions")]
        public Version PlatformVersion { get; set; }

        public ProjectPlatformSettings()
        {
            Language = GenerationTargetLanguage.CSharp;
#pragma warning disable 618
            LanguageVersion = new Version(3, 0);
            Platform = GenerationTargetPlatform.DotNet;
            PlatformVersion = new Version(3, 5);
#pragma warning restore 618
        }
    }
}