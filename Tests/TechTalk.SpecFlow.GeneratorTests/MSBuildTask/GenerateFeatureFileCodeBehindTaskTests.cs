using FluentAssertions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Moq;
using SpecFlow.Tools.MsBuild.Generation;
using System.Collections.Generic;
using BoDi;
using TechTalk.SpecFlow.Analytics;
using Xunit;
using Xunit.Abstractions;

namespace TechTalk.SpecFlow.GeneratorTests.MSBuildTask
{
    public class GenerateFeatureFileCodeBehindTaskTests
    {
        private readonly ITestOutputHelper _output;

        public GenerateFeatureFileCodeBehindTaskTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private Mock<IFeatureFileCodeBehindGenerator> GetFeatureFileCodeBehindGeneratorMock()
        {
            var generatorMock = new Mock<IFeatureFileCodeBehindGenerator>();
            generatorMock
                .Setup(m => m.GenerateFilesForProject(
                    It.IsAny<List<string>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new List<string>());
            return generatorMock;
        }

        private Mock<IAnalyticsTransmitter> GetAnalyticsTransmitterMock()
        {
            var analyticsTransmitterMock = new Mock<IAnalyticsTransmitter>();
            analyticsTransmitterMock.Setup(at => at.TransmitSpecflowProjectCompilingEvent(It.IsAny<SpecFlowProjectCompilingEvent>()))
                .Callback(() => { });
            return analyticsTransmitterMock;
        }

        [Fact]
        public void Execute_OnlyRequiredPropertiesAreSet_ShouldWork()
        {
            //ARRANGE
            var generateFeatureFileCodeBehindTask = new GenerateFeatureFileCodeBehindTask
            {
                ProjectPath = "ProjectPath.csproj",
                BuildEngine = new MockBuildEngine(_output),
                CodeBehindGenerator = GetFeatureFileCodeBehindGeneratorMock().Object,
                AnalyticsTransmitter = GetAnalyticsTransmitterMock().Object
            };

            //ACT
            var result = generateFeatureFileCodeBehindTask.Execute();

            //ASSERT
            result.Should().BeTrue();
        }

        [Fact]
        public void Execute_AllPropertiesAreSet_ShouldWork()
        {
            //ARRANGE
            var generateFeatureFileCodeBehindTask = new GenerateFeatureFileCodeBehindTask
            {
                RootNamespace = "RootNamespace",
                ProjectPath = "ProjectPath.csproj",
                FeatureFiles = new TaskItem[0],
                GeneratorPlugins = new TaskItem[0],
                BuildEngine = new MockBuildEngine(_output),
                CodeBehindGenerator = GetFeatureFileCodeBehindGeneratorMock().Object,
                AnalyticsTransmitter = GetAnalyticsTransmitterMock().Object
            };

            //ACT
            var result = generateFeatureFileCodeBehindTask.Execute();

            //ASSERT
            result.Should().BeTrue();
        }

        [Fact]
        public void Execute_FeatureFilesNotSet_ShouldWork()
        {
            //ARRANGE
            var generateFeatureFileCodeBehindTask = new GenerateFeatureFileCodeBehindTask
            {
                RootNamespace = "RootNamespace",
                ProjectPath = "ProjectPath.csproj",
                GeneratorPlugins = new TaskItem[0],
                BuildEngine = new MockBuildEngine(_output),
                CodeBehindGenerator = GetFeatureFileCodeBehindGeneratorMock().Object,
                AnalyticsTransmitter = GetAnalyticsTransmitterMock().Object
            };

            //ACT
            var result = generateFeatureFileCodeBehindTask.Execute();

            //ASSERT
            result.Should().BeTrue();
        }

        [Fact]
        public void Execute_GeneratorPluginsNotSet_ShouldWork()
        {
            //ARRANGE
            var generateFeatureFileCodeBehindTask = new GenerateFeatureFileCodeBehindTask
            {
                RootNamespace = "RootNamespace",
                ProjectPath = "ProjectPath.csproj",
                FeatureFiles = new TaskItem[0],
                BuildEngine = new MockBuildEngine(_output),
                CodeBehindGenerator = GetFeatureFileCodeBehindGeneratorMock().Object,
                AnalyticsTransmitter = GetAnalyticsTransmitterMock().Object
            };

            //ACT
            var result = generateFeatureFileCodeBehindTask.Execute();

            //ASSERT
            result.Should().BeTrue();
        }

        [Fact]
        public void Should_TryToSendAnalytics()
        {
            //ARRANGE
            var analyticsTransmitterMock = GetAnalyticsTransmitterMock();
            var generateFeatureFileCodeBehindTask = new GenerateFeatureFileCodeBehindTask
            {
                ProjectPath = "ProjectPath.csproj",
                BuildEngine = new MockBuildEngine(_output),
                CodeBehindGenerator = GetFeatureFileCodeBehindGeneratorMock().Object,
                AnalyticsTransmitter = analyticsTransmitterMock.Object
            };

            //ACT
            bool result = generateFeatureFileCodeBehindTask.Execute();

            //ASSERT
            result.Should().BeTrue();
            analyticsTransmitterMock.Verify(sink => sink.TransmitSpecflowProjectCompilingEvent(It.IsAny<SpecFlowProjectCompilingEvent>()), Times.Once);
        }
    }
}