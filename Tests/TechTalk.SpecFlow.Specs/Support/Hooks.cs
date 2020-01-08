using System;
using System.IO;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.Helpers;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Specs.Support
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly CurrentVersionDriver _currentVersionDriver;
        private readonly RuntimeInformationProvider _runtimeInformationProvider;
        private readonly IUnitTestRuntimeProvider _unitTestRuntimeProvider;
        private readonly TestProjectFolders _testProjectFolders;

        public Hooks(ScenarioContext scenarioContext, CurrentVersionDriver currentVersionDriver, RuntimeInformationProvider runtimeInformationProvider, IUnitTestRuntimeProvider unitTestRuntimeProvider, TestProjectFolders testProjectFolders)
        {
            _scenarioContext = scenarioContext;
            _currentVersionDriver = currentVersionDriver;
            _runtimeInformationProvider = runtimeInformationProvider;
            _unitTestRuntimeProvider = unitTestRuntimeProvider;
            _testProjectFolders = testProjectFolders;
        }

        [BeforeScenario("WindowsOnly")]
        public void SkipWindowsOnlyScenarioIfNotOnWindows()
        {
            if (!_runtimeInformationProvider.IsOperatingSystemWindows())
            {
                _unitTestRuntimeProvider.TestIgnore("Test must be run on a Windows host system.");
            }
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _currentVersionDriver.NuGetVersion = NuGetPackageVersion.Version;
            _currentVersionDriver.SpecFlowNuGetVersion = NuGetPackageVersion.Version;
            _scenarioContext.ScenarioContainer.RegisterTypeAs<OutputConnector, IOutputWriter>();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            if (_scenarioContext.TestError == null && _testProjectFolders.IsPathToSolutionFileSet)
            {
                try
                {
                    FileSystemHelper.DeleteFolder(_testProjectFolders.PathToSolutionDirectory);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
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

            var currentVersionDriver = new CurrentVersionDriver {NuGetVersion = NuGetPackageVersion.Version };

            string[] packageNames = { "SpecFlow", "SpecFlow.CustomPlugin", "SpecFlow.MsTest", "SpecFlow.NUnit", "SpecFlow.NUnit.Runners", "SpecFlow.Tools.MsBuild.Generation", "SpecFlow.xUnit" };
            
            foreach (var name in packageNames)
            {
                string hooksPath = Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), ".nuget", "packages", name, currentVersionDriver.NuGetVersion);
                FileSystemHelper.DeleteFolder(hooksPath);
            }
        }
    }
}
