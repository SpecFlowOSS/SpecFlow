using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.CucumberMessages;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class PlatformFactoryTests
    {
        [Fact(DisplayName = @"BuildFromSystemInformation should return a success with the system's CPU architecture")]
        public void BuildFromSystemInformation_CpuArchitecture_ShouldReturnSuccessWithSystemCpuArchitecture()
        {
            // ARRANGE
            const string cpuArchitecture = "x86";

            var systemInformationProviderMock = GetSystemInformationProviderMock(cpuArchitecture: cpuArchitecture);
            var specFlowVersionInformationProvider = GetSpecFlowVersionInformationProvider();

            var platformFactory = new PlatformFactory(systemInformationProviderMock.Object, specFlowVersionInformationProvider.Object);

            // ACT
            var result = platformFactory.BuildFromSystemInformation();

            // ASSERT
            result.Cpu.Should().Be(cpuArchitecture);
        }

        [Fact(DisplayName = @"BuildFromSystemInformation should return a success with the system's operating system")]
        public void BuildFromSystemInformation_OperatingSystem_ShouldReturnSuccessWithOperatingSystem()
        {
            // ARRANGE
            const string operatingSystem = "Ubuntu";

            var systemInformationProviderMock = GetSystemInformationProviderMock(operatingSystem: operatingSystem);
            var specFlowVersionInformationProvider = GetSpecFlowVersionInformationProvider();

            var platformFactory = new PlatformFactory(systemInformationProviderMock.Object, specFlowVersionInformationProvider.Object);

            // ACT
            var result = platformFactory.BuildFromSystemInformation();

            // ASSERT
            result.Os.Should().Be(operatingSystem);
        }

        [Fact(DisplayName = @"BuildFromSystemInformation should return a success with the current SpecFlow version")]
        public void BuildFromSystemInformation_PackageVersion_ShouldReturnSuccessWithVersion()
        {
            // ARRANGE
            const string packageVersion = "3.1.0";

            var systemInformationProviderMock = GetSystemInformationProviderMock();
            var specFlowVersionInformationProvider = GetSpecFlowVersionInformationProvider(packageVersion);

            var platformFactory = new PlatformFactory(systemInformationProviderMock.Object, specFlowVersionInformationProvider.Object);

            // ACT
            var result = platformFactory.BuildFromSystemInformation();

            // ASSERT
            result.Version.Should().Be(packageVersion);
        }

        [Fact(DisplayName = @"BuildFromSystemInformation should return a success with SpecFlow as implementation")]
        public void BuildFromSystemInformation_ShouldReturnSuccessWithSpecFlowAsImplementation()
        {
            // ARRANGE
            const string expectedImplementation = "SpecFlow";

            var systemInformationProviderMock = GetSystemInformationProviderMock();
            var specFlowVersionInformationProvider = GetSpecFlowVersionInformationProvider();

            var platformFactory = new PlatformFactory(systemInformationProviderMock.Object, specFlowVersionInformationProvider.Object);

            // ACT
            var result = platformFactory.BuildFromSystemInformation();

            // ASSERT
            result.Implementation.Should().Be(expectedImplementation);
        }

        public Mock<ISpecFlowVersionInformationProvider> GetSpecFlowVersionInformationProvider(string packageVersion = "3.1.0")
        {
            var specFlowVersionInformationProvider = new Mock<ISpecFlowVersionInformationProvider>();
            specFlowVersionInformationProvider.Setup(m => m.GetAssemblyVersion())
                                              .Returns(packageVersion);
            return specFlowVersionInformationProvider;
        }

        public Mock<ISystemInformationProvider> GetSystemInformationProviderMock(string cpuArchitecture = "x64", string operatingSystem = "Windows")
        {
            var systemInformationProviderMock = new Mock<ISystemInformationProvider>();
            systemInformationProviderMock.Setup(m => m.GetCpuArchitecture())
                                         .Returns(cpuArchitecture);
            systemInformationProviderMock.Setup(m => m.GetOperatingSystem())
                                         .Returns(operatingSystem);
            return systemInformationProviderMock;
        }
    }
}
