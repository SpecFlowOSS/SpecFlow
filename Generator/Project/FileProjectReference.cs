using System;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class FileProjectReference : IProjectReference
    {
        public string ProjectFilePath { get; set; }

        public FileProjectReference(string projectFilePath)
        {
            ProjectFilePath = projectFilePath;
        }

        static public FileProjectReference AssertFileProjectReference(IProjectReference projectReference)
        {
            return (FileProjectReference)projectReference; //TODO: better error handling
        }
    }
}