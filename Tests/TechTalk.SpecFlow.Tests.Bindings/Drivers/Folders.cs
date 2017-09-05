using System;
using System.IO;
using System.Reflection;

namespace TechTalk.SpecFlow.Tests.Bindings.Drivers
{
    public class Folders
    {
        private string _packageFolder;
        private string _sourceRoot;


        public string TestFolder => Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        public string SourceRoot
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_sourceRoot))
                    return _sourceRoot;
                return Path.GetFullPath(Path.Combine(TestFolder, "..", "..", "..", "..", ".."));
            }
            set => _sourceRoot = value;
        }


        public string PackageFolder
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_packageFolder))
                    return _packageFolder;

                return Path.GetFullPath(Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), ".nuget", "packages"));
            }
            set => _packageFolder = value;
        }
    }
}