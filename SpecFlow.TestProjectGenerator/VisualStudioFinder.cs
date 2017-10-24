using System;
using System.IO;
using System.Linq;

namespace SpecFlow.TestProjectGenerator
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
            string vsWherePath = Path.Combine(_folders.GlobalPackages, "vswhere", "2.2.7", "tools", "vswhere.exe");

            if (!File.Exists(vsWherePath))
            {
                throw new FileNotFoundException("vswhere can not be found! Is the version number correct?", vsWherePath);
            }

            var ph = new ProcessHelper();
            ph.RunProcess(vsWherePath, VsWhereParameter);
            var output = ph.ConsoleOutput;


            var lines = output.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
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