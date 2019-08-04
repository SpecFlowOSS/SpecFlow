using System;
using System.IO;
using TechTalk.SpecFlow.Specs.MSBuild.Support;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.Helpers;

namespace TechTalk.SpecFlow.Specs.MSBuild.Hooks
{
    [Binding]
    public class FolderManagement
    {
        [BeforeTestRun]
        public static void CleanWorkingDirectories()
        {
            var appConfigDriver = new AppConfigDriver();
            var folders = new Folders(appConfigDriver);


            DeletePackageVersionFolders();
            DeleteOldTestRunData(folders);
        }

        private static void DeleteOldTestRunData(Folders folders)
        {
            try
            {
                FileSystemHelper.DeleteFolderContent(folders.FolderToSaveGeneratedSolutions);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void DeletePackageVersionFolders()
        {

            var currentVersionDriver = new CurrentVersionDriver { NuGetVersion = NuGetPackageInfo.Version };

            string[] packageNames = { "SpecFlow", "SpecFlow.CustomPlugin", "SpecFlow.MsTest", "SpecFlow.NUnit", "SpecFlow.NUnit.Runners", "SpecFlow.Tools.MsBuild.Generation", "SpecFlow.xUnit" };

            foreach (var name in packageNames)
            {
                string hooksPath = Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), ".nuget", "packages", name, currentVersionDriver.NuGetVersion);
                FileSystemHelper.DeleteFolder(hooksPath);
            }
        }
    }
}
