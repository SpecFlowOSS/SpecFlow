using FluentAssertions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Moq;
using SpecFlow.Tools.MsBuild.Generation;
using System.Collections.Generic;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests.MSBuildTask
{
    public class GenerateFeatureFileCodeBehindTaskTests
    {
        [Fact]
        public void Execute_OnlyRequiredPropertiesAreSet_ShouldWork()
        {
            //ARRANGE
            var generateFeatureFileCodeBehindMock = new Mock<IGenerateFeatureFileCodeBehind>();
            generateFeatureFileCodeBehindMock.Setup(m => m.GenerateFilesForProject(It.IsAny<List<string>>(),
                                                                                   It.IsAny<string>(),
                                                                                   It.IsAny<string>(),
                                                                                   It.IsAny<string>(),
                                                                                   It.IsAny<string>(),
                                                                                   It.IsAny<List<string>>()))
                                             .Returns(new List<string>());


            var generateFeatureFileCodeBehindTask = new GenerateFeatureFileCodeBehindTask(() => generateFeatureFileCodeBehindMock.Object)
            {
                RootNamespace = "RootNamespace",
                ProjectPath = "ProjectPath",
                BuildEngine = new Mock<IBuildEngine>().Object
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
            var generateFeatureFileCodeBehindMock = new Mock<IGenerateFeatureFileCodeBehind>();
            generateFeatureFileCodeBehindMock.Setup(m => m.GenerateFilesForProject(It.IsAny<List<string>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>()))
                .Returns(new List<string>());

            var generateFeatureFileCodeBehindTask = new GenerateFeatureFileCodeBehindTask(() => generateFeatureFileCodeBehindMock.Object)
            {
                RootNamespace = "RootNamespace",
                ProjectPath = "ProjectPath",
                FeatureFiles = new TaskItem[0],
                GeneratorPlugins = new TaskItem[0],
                BuildEngine = new Mock<IBuildEngine>().Object
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
            var generateFeatureFileCodeBehindMock = new Mock<IGenerateFeatureFileCodeBehind>();
            generateFeatureFileCodeBehindMock.Setup(m => m.GenerateFilesForProject(It.IsAny<List<string>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>()))
                .Returns(new List<string>());

            var generateFeatureFileCodeBehindTask = new GenerateFeatureFileCodeBehindTask(() => generateFeatureFileCodeBehindMock.Object)
            {
                RootNamespace = "RootNamespace",
                ProjectPath = "ProjectPath",
                GeneratorPlugins = new TaskItem[0],
                BuildEngine = new Mock<IBuildEngine>().Object
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
            var generateFeatureFileCodeBehindMock = new Mock<IGenerateFeatureFileCodeBehind>();
            generateFeatureFileCodeBehindMock.Setup(m => m.GenerateFilesForProject(It.IsAny<List<string>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>()))
                .Returns(new List<string>());

            var generateFeatureFileCodeBehindTask = new GenerateFeatureFileCodeBehindTask(() => generateFeatureFileCodeBehindMock.Object)
            {
                RootNamespace = "RootNamespace",
                ProjectPath = "ProjectPath",
                FeatureFiles = new TaskItem[0],
                BuildEngine = new Mock<IBuildEngine>().Object
            };

            //ACT
            var result = generateFeatureFileCodeBehindTask.Execute();

            //ASSERT
            result.Should().BeTrue();
        }
    }
}