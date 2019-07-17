using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class PlatformFactory : IPlatformFactory
    {
        private readonly ISystemInformationProvider _systemInformationProvider;
        private readonly ISpecFlowVersionInformationProvider _specFlowVersionInformationProvider;

        public PlatformFactory(ISystemInformationProvider systemInformationProvider, ISpecFlowVersionInformationProvider specFlowVersionInformationProvider)
        {
            _systemInformationProvider = systemInformationProvider;
            _specFlowVersionInformationProvider = specFlowVersionInformationProvider;
        }

        public TestCaseStarted.Types.Platform BuildFromSystemInformation()
        {
            var platform = new TestCaseStarted.Types.Platform
            {
                Cpu = _systemInformationProvider.GetCpuArchitecture(),
                Os = _systemInformationProvider.GetOperatingSystem(),
                Implementation = "SpecFlow",
                Version = _specFlowVersionInformationProvider.GetAssemblyVersion()
            };

            return platform;
        }
    }
}
