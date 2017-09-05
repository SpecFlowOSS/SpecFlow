using System;
using System.IO;
using System.Linq;

namespace TechTalk.SpecFlow.Tests.Bindings.Drivers
{
    public class VisualStudioFinder
    {
        private const string VsWhereParameter = @"-latest -products * -requires Microsoft.VisualStudio.PackageGroup.TestTools.Core -property installationPath";
        private readonly Folders _folders;

        public VisualStudioFinder(Folders folders)
        {
            _folders = folders;
        }

        public string Find()
        {
            var vsWherePath = Path.Combine(_folders.SourceRoot, "Binaries", "vswhere.exe");

            var ph = new ProcessHelper();
            ph.RunProcess(vsWherePath, VsWhereParameter);
            var output = ph.ConsoleOutput;


            var lines = output.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            return lines.First();
        }

        public string FindMSBuild()
        {
            var msbuildExe = Path.Combine(Find(), "MSBuild", "15.0", "Bin", "msbuild.exe");
            return msbuildExe;
        }

        public string FindDevEnv()
        {
            return Path.Combine(Find(), "Common7", "IDE", "devenv.exe");
        }
    }
}