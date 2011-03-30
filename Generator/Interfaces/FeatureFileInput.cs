using System;
using System.IO;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    [Serializable]
    public class FeatureFileInput
    {
        public string FullPath { get; private set; }
        public string ProjectRelativePath { get; private set; }
        public string CustomNamespace { get; private set; }
        public TextReader ContentReader { get; private set; }

        public FeatureFileInput(string fullPath, string projectRelativePath, string customNamespace, TextReader contentReader)
        {
            if (contentReader == null) throw new ArgumentNullException("contentReader");

            FullPath = fullPath;
            ProjectRelativePath = projectRelativePath;
            CustomNamespace = customNamespace;
            ContentReader = contentReader;
        }
    }
}