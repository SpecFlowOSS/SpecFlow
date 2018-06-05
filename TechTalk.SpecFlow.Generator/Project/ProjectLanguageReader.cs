using System;
using System.IO;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class ProjectLanguageReader
    {
        public string GetLanguage(string projectFileName)
        {
            string projFileExtension = Path.GetExtension(projectFileName);
            switch (projFileExtension.ToUpper())
            {
                case ".CSPROJ": return GenerationTargetLanguage.CSharp;
                case ".VBPROJ": return GenerationTargetLanguage.VB;
                default: throw new InvalidOperationException($"Unknown project format: {projFileExtension}");
            }
        }
    }
}