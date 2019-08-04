using TechTalk.SpecFlow.Specs.MSBuild.Support;
using TechTalk.SpecFlow.TestProjectGenerator;

namespace TechTalk.SpecFlow.Specs.MSBuild.Hooks
{
    [Binding]
    public class VersionSetup
    {
        private readonly CurrentVersionDriver _currentVersionDriver;

        public VersionSetup(CurrentVersionDriver currentVersionDriver)
        {
            _currentVersionDriver = currentVersionDriver;
        }

        [BeforeScenario]
        public void SetVersionInformation()
        {
            _currentVersionDriver.NuGetVersion = NuGetPackageInfo.Version;
            _currentVersionDriver.SpecFlowNuGetVersion = NuGetPackageInfo.Version;
        }
    }
}
