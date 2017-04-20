using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator.Project;

namespace TechTalk.SpecFlow.GeneratorTests
{
    //If this tests are failing in R# TestRunner, disable shadow copy of assemblies
    [TestFixture]
    public class MsBuildProjectReaderTests
    {

        [Test]
        public void Should_parse_ToolsVersion4_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly("Data\\CSProj_ToolsVersion_4\\sampleCsProjectfile.csproj");
        }

        [Test]
        public void Should_parse_ToolsVersion14_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly("Data\\CSProj_ToolsVersion_14\\sampleCsProjectfile.csproj");
        }

        [Test]
        public void Should_parse_ToolsVersion12_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly("Data\\CSProj_ToolsVersion_12\\sampleCsProjectfile.csproj");
        }

        [Test]
        public void Should_parse_CSProj_New_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly("Data\\CSProj_New\\sampleCsProjectfile.csproj");
        }

        [Test]
        public void Should_parse_CSProj_NewComplex_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly("Data\\CSProj_NewComplex\\sampleCsProjectfile.csproj");
        }

        private void Should_parse_csproj_file_correctly(string csprojPath)
        {
            var directoryName = Path.GetDirectoryName(this.GetType().Assembly.Location);
            string projectFilePath = Path.Combine(directoryName, csprojPath);
            SpecFlowProject specflowProjectfile = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectFilePath);
            specflowProjectfile.FeatureFiles.Count.Should().Be(3);
            specflowProjectfile.FeatureFiles.Single(x => x.ProjectRelativePath == @"Features\Login\SocialLogins.feature").Should().NotBeNull();
            specflowProjectfile.FeatureFiles.Single(x => x.ProjectRelativePath == @"Features\WorkflowDefinition\CreateWorkflowDefinition.feature").Should().NotBeNull();
            specflowProjectfile.FeatureFiles.Single(x => x.ProjectRelativePath == @"Features\WorkflowInstance\WorkflowInstance.feature").Should().NotBeNull();

            specflowProjectfile.ProjectSettings.AssemblyName.Should().Be("Hacapp.Web.Tests.UI");
            specflowProjectfile.ProjectSettings.DefaultNamespace.Should().Be("Hacapp.Web.Tests.UI");
            specflowProjectfile.ProjectSettings.ProjectName.Should().Be("sampleCsProjectfile");

            specflowProjectfile.ProjectSettings.ProjectPlatformSettings.Language.Should().Be("C#");
            specflowProjectfile.ProjectSettings.ProjectPlatformSettings.LanguageVersion.Should().Be(new Version(3, 0));
            specflowProjectfile.ProjectSettings.ProjectPlatformSettings.Platform.Should().Be(".NET");
            specflowProjectfile.ProjectSettings.ProjectPlatformSettings.PlatformVersion.Should().Be(new Version(3, 5));

            specflowProjectfile.Configuration.GeneratorConfiguration.AllowDebugGeneratedFiles.Should().BeFalse();
            specflowProjectfile.Configuration.GeneratorConfiguration.AllowRowTests.Should().BeTrue();
            specflowProjectfile.Configuration.GeneratorConfiguration.GeneratorUnitTestProvider.Should().Be("MSTest");
            specflowProjectfile.Configuration.GeneratorConfiguration.FeatureLanguage.Name.Should().Be("en-US");
        }
    }
}