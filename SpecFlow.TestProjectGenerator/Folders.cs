using System;
using System.IO;
using System.Reflection;

namespace SpecFlow.TestProjectGenerator
{
    public class Folders
    {
        protected string _nugetFolder;
        protected string _packageFolder;
        protected string _sourceRoot;
        protected string _specFlow;
        protected string _vsAdapterFolder;

        protected bool _vsAdapterFolderChanged;

        public string TestFolder => Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        public virtual string SourceRoot
        {
            get => !string.IsNullOrWhiteSpace(_sourceRoot) ? _sourceRoot : Path.GetFullPath(Path.Combine(TestFolder, "..", "..", ".."));
            set => _sourceRoot = value;
        }

        public string VSAdapterFolder
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_vsAdapterFolder) ? _vsAdapterFolder : VsAdapterFolderProjectBinaries;
            }
            set
            {
                _vsAdapterFolder = value;
                _vsAdapterFolderChanged = true;
            }
        }

        public virtual string VsAdapterFolderProjectBinaries => throw new NotImplementedException();

        public bool VsAdapterFolderChanged => _vsAdapterFolderChanged;


        public virtual string NuGetFolder
        {
            get => !string.IsNullOrWhiteSpace(_nugetFolder) ? _nugetFolder : Path.GetFullPath(Path.Combine(SourceRoot, "Installer", "NuGetPackages", "bin", "Debug"));
            set => _nugetFolder = value;
        }

        public string PackageFolder
        {
            get => !string.IsNullOrWhiteSpace(_packageFolder) ? _packageFolder : Path.GetFullPath(Path.Combine(SourceRoot, "packages"));
            set => _packageFolder = value;
        }

        public string GlobalPackages => Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), ".nuget", "packages");

        public string SpecFlow
        {
            get => !string.IsNullOrWhiteSpace(_specFlow) ? _specFlow : Path.GetFullPath(TestFolder);
            set => _specFlow = value;
        }

        public virtual string TestProjectRootFolder => Path.Combine(Path.GetTempPath(), "SpecFlow");
    }
}