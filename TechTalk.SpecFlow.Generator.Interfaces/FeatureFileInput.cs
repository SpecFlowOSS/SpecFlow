using System;
using System.IO;

namespace TechTalk.SpecFlow.Generator
{
    [Serializable]
    public class FeatureFileInput
    {
        public string FullPath { get; private set; }
        public string ProjectRelative { get; private set; }
        public string CustomNamespace { get; private set; }
        public TextReader ContentReader { get; private set; }

        public FeatureFileInput(string fullPath, string projectRelative, string customNamespace, TextReader contentReader)
        {
            FullPath = fullPath;
            ProjectRelative = projectRelative;
            CustomNamespace = customNamespace;
            ContentReader = contentReader;
        }
    }
}