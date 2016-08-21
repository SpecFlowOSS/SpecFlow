using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class MsBuildProjectReaderTests
    {
        [Test]
        public void Should_parse_csproj_file_correctly()
        {
            var directoryName = Path.GetDirectoryName(this.GetType().Assembly.Location);
            string projectFilePath = Path.Combine(directoryName, "Data\\sampleCsProjectfile.csproj");
            SpecFlowProject specflowProjectfile = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectFilePath, new NullListener());
            specflowProjectfile.FeatureFiles.Count.Should().Be(3);
            specflowProjectfile.FeatureFiles.Single(x => x.ProjectRelativePath == @"Features\Login\SocialLogins.feature").Should().NotBeNull();
            specflowProjectfile.FeatureFiles.Single(x => x.ProjectRelativePath == @"Features\WorkflowDefinition\CreateWorkflowDefinition.feature").Should().NotBeNull();
            specflowProjectfile.FeatureFiles.Single(x => x.ProjectRelativePath == @"Features\WorkflowInstance\WorkflowInstance.feature").Should().NotBeNull();

            specflowProjectfile.ProjectSettings.AssemblyName.Should().Be("Hacapp.Web.Tests.UI");
            specflowProjectfile.ProjectSettings.DefaultNamespace.Should().Be("Hacapp.Web.Tests.UI");
            specflowProjectfile.ProjectSettings.ProjectName.Should().Be("sampleCsProjectfile");

            specflowProjectfile.ProjectSettings.ProjectPlatformSettings.Language.Should().Be("C#");
            specflowProjectfile.ProjectSettings.ProjectPlatformSettings.LanguageVersion.Should().Be(new Version(3,0));
            specflowProjectfile.ProjectSettings.ProjectPlatformSettings.Platform.Should().Be(".NET");
            specflowProjectfile.ProjectSettings.ProjectPlatformSettings.PlatformVersion.Should().Be(new Version(3,5));

            specflowProjectfile.Configuration.GeneratorConfiguration.AllowDebugGeneratedFiles.Should().BeFalse();
            specflowProjectfile.Configuration.GeneratorConfiguration.AllowRowTests.Should().BeTrue();
            specflowProjectfile.Configuration.GeneratorConfiguration.GeneratorUnitTestProvider.Should().Be("MSTest");
            specflowProjectfile.Configuration.GeneratorConfiguration.FeatureLanguage.Name.Should().Be("en-US");
        }
    }
}