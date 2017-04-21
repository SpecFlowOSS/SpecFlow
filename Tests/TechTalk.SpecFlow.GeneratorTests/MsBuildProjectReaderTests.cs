using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Project;

namespace TechTalk.SpecFlow.GeneratorTests
{
    //If this tests are failing in R# TestRunner, disable shadow copy of assemblies
    [TestFixture]
    public class MsBuildProjectReaderTests
    {
        private void Should_parse_csproj_file_correctly(string csprojPath, string language, string assemblyName, string rootNamespace, string projectName)
        {
            var directoryName = Path.GetDirectoryName(GetType().Assembly.Location);
            var projectFilePath = Path.Combine(directoryName, csprojPath);
            var specflowProjectfile = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectFilePath);


            specflowProjectfile.ProjectSettings.AssemblyName.Should().Be(assemblyName);
            specflowProjectfile.ProjectSettings.DefaultNamespace.Should().Be(rootNamespace);
            specflowProjectfile.ProjectSettings.ProjectName.Should().Be(projectName);

            specflowProjectfile.ProjectSettings.ProjectPlatformSettings.Language.Should().Be(language);

            specflowProjectfile.FeatureFiles.Count.Should().Be(4);
            specflowProjectfile.FeatureFiles.Single(x => x.ProjectRelativePath == @"Features\Login\SocialLogins.feature").Should().NotBeNull();
            specflowProjectfile.FeatureFiles.Single(x => x.ProjectRelativePath == @"Features\WorkflowDefinition\CreateWorkflowDefinition.feature").Should().NotBeNull();
            specflowProjectfile.FeatureFiles.Single(x => x.ProjectRelativePath == @"Features\WorkflowDefinition\CreateWorkflowDefinition.feature").CustomNamespace.Should().Be("CustomNameSpace");
            specflowProjectfile.FeatureFiles.Single(x => x.ProjectRelativePath == @"Features\WorkflowInstance\WorkflowInstance.feature").Should().NotBeNull();
            specflowProjectfile.FeatureFiles.Single(x => x.ProjectRelativePath == @"..\..\LinkedFeature.feature").Should().NotBeNull();


            specflowProjectfile.Configuration.GeneratorConfiguration.AllowDebugGeneratedFiles.Should().BeFalse();
            specflowProjectfile.Configuration.GeneratorConfiguration.AllowRowTests.Should().BeTrue();
            specflowProjectfile.Configuration.GeneratorConfiguration.GeneratorUnitTestProvider.Should().Be("MSTest");
            specflowProjectfile.Configuration.GeneratorConfiguration.FeatureLanguage.Name.Should().Be("en-US");
        }

        [Test]
        public void Should_parse_CSProj_New_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly("Data\\CSProj_New\\sampleCsProjectfile.csproj", GenerationTargetLanguage.CSharp, "sampleCsProjectfile", "sampleCsProjectfile", "sampleCsProjectfile");
        }

        [Test]
        public void Should_parse_CSProj_NewComplex_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly("Data\\CSProj_NewComplex\\sampleCsProjectfile.csproj", GenerationTargetLanguage.CSharp, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }

        [Test]
        public void Should_parse_ToolsVersion12_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly("Data\\CSProj_ToolsVersion_12\\sampleCsProjectfile.csproj", GenerationTargetLanguage.CSharp, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }

        [Test]
        public void Should_parse_ToolsVersion12_vbproj_file_correctly()
        {
            Should_parse_csproj_file_correctly("Data\\VBProj_ToolsVersion_12\\sampleCsProjectfile.vbproj", GenerationTargetLanguage.VB, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }

        [Test]
        public void Should_parse_ToolsVersion14_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly("Data\\CSProj_ToolsVersion_14\\sampleCsProjectfile.csproj", GenerationTargetLanguage.CSharp, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }

        [Test]
        public void Should_parse_ToolsVersion14_vbproj_file_correctly()
        {
            Should_parse_csproj_file_correctly("Data\\VBProj_ToolsVersion_14\\sampleCsProjectfile.vbproj", GenerationTargetLanguage.VB, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }

        [Test]
        public void Should_parse_ToolsVersion4_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly("Data\\CSProj_ToolsVersion_4\\sampleCsProjectfile.csproj", GenerationTargetLanguage.CSharp, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }

        [Test]
        public void Should_parse_ToolsVersion4_vbproj_file_correctly()
        {
            Should_parse_csproj_file_correctly("Data\\VBProj_ToolsVersion_4\\sampleCsProjectfile.vbproj", GenerationTargetLanguage.VB, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }
    }
}