using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Analytics;
using TechTalk.SpecFlow.Analytics.UserId;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.EnvironmentAccess;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Analytics
{
    public class AnalyticsEventProviderTests
    {
        [Fact]
        public void Should_return_the_build_server_name_in_Compiling_Event()
        {
            var userUniqueIdStoreMock = new Mock<IUserUniqueIdStore>();
            var environmentMock = new Mock<IEnvironmentWrapper>();
            var sut = new AnalyticsEventProvider(userUniqueIdStoreMock.Object, new UnitTestProvider.UnitTestProviderConfiguration(), environmentMock.Object);

            environmentMock
                .Setup(m => m.GetEnvironmentVariable("TF_BUILD"))
                .Returns(new Success<string>("true"));

            var compilingEvent = sut.CreateProjectCompilingEvent(null, null, null, null, null);
            
            compilingEvent.BuildServerName.Should().Be("Azure Pipelines");
        }

        [Fact]
        public void Should_return_the_build_server_name_in_Running_Event()
        {
            var userUniqueIdStoreMock = new Mock<IUserUniqueIdStore>();
            var environmentMock = new Mock<IEnvironmentWrapper>();
            var sut = new AnalyticsEventProvider(userUniqueIdStoreMock.Object, new UnitTestProvider.UnitTestProviderConfiguration(), environmentMock.Object);

            environmentMock
                .Setup(m => m.GetEnvironmentVariable("TEAMCITY_VERSION"))
                .Returns(new Success<string>("true"));

            var compilingEvent = sut.CreateProjectRunningEvent(null);
            
            compilingEvent.BuildServerName.Should().Be("TeamCity");
        }

        [Fact]
        public void Should_return_null_for_the_build_server_name_when_not_detected()
        {
            var userUniqueIdStoreMock = new Mock<IUserUniqueIdStore>();
            var environmentMock = new Mock<IEnvironmentWrapper>();
            var sut = new AnalyticsEventProvider(userUniqueIdStoreMock.Object, new UnitTestProvider.UnitTestProviderConfiguration(), environmentMock.Object);

            var compilingEvent = sut.CreateProjectRunningEvent(null);
            
            compilingEvent.BuildServerName.Should().Be(null);
        }
    }
}
