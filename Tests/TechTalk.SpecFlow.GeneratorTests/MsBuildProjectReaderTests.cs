﻿using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Generator.Helpers;

namespace TechTalk.SpecFlow.GeneratorTests
{
    
    public class MsBuildProjectReaderTests
    {
        private void Should_parse_csproj_file_correctly(string csprojPath, string language, string assemblyName, string rootNamespace, string projectName)
        {
            var directoryName = Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).LocalPath);
            var projectFilePath = Path.Combine(directoryName, csprojPath);
            var specflowProjectfile = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectFilePath);


            specflowProjectfile.ProjectSettings.AssemblyName.Should().Be(assemblyName);
            specflowProjectfile.ProjectSettings.DefaultNamespace.Should().Be(rootNamespace);
            specflowProjectfile.ProjectSettings.ProjectName.Should().Be(projectName);

            specflowProjectfile.ProjectSettings.ProjectPlatformSettings.Language.Should().Be(language);

            specflowProjectfile.FeatureFiles.Count.Should().Be(6);
            specflowProjectfile.FeatureFiles.Should().ContainSingle(x => x.ProjectRelativePath == PathHelper.SanitizeDirectorySeparatorChar(@"Features\Login\SocialLogins.feature"));
            specflowProjectfile.FeatureFiles.Should().ContainSingle(x => x.ProjectRelativePath == PathHelper.SanitizeDirectorySeparatorChar(@"Features\WorkflowDefinition\CreateWorkflowDefinition.feature"));
            specflowProjectfile.FeatureFiles.Should().ContainSingle(x => x.ProjectRelativePath == PathHelper.SanitizeDirectorySeparatorChar(@"Features\WorkflowDefinition\CreateWorkflowDefinition.feature") && x.CustomNamespace == "CustomNameSpace");
            specflowProjectfile.FeatureFiles.Should().ContainSingle(x => x.ProjectRelativePath == PathHelper.SanitizeDirectorySeparatorChar(@"Features\WorkflowInstance\WorkflowInstance.feature"));
            specflowProjectfile.FeatureFiles.Should().ContainSingle(x => x.ProjectRelativePath == PathHelper.SanitizeDirectorySeparatorChar(@"..\..\LinkedFeature.feature"));
            specflowProjectfile.FeatureFiles.Should().ContainSingle(x => x.ProjectRelativePath == PathHelper.SanitizeDirectorySeparatorChar(@"..\ExampleFeatures\Features\Subfolder1\ExternalFeature1.feature"));
            specflowProjectfile.FeatureFiles.Should().ContainSingle(x => x.ProjectRelativePath == PathHelper.SanitizeDirectorySeparatorChar(@"..\ExampleFeatures\Features\Subfolder2\ExternalFeature2.feature"));
            

            specflowProjectfile.Configuration.SpecFlowConfiguration.AllowDebugGeneratedFiles.Should().BeFalse();
            specflowProjectfile.Configuration.SpecFlowConfiguration.AllowRowTests.Should().BeTrue();
            specflowProjectfile.Configuration.SpecFlowConfiguration.UnitTestProvider.Should().Be("MSTest");
            specflowProjectfile.Configuration.SpecFlowConfiguration.FeatureLanguage.Name.Should().Be("en-US");
        }

        [Fact]
        public void Should_parse_CSProj_New_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly(PathHelper.SanitizeDirectorySeparatorChar(@"Data\CSProj_New\sampleCsProjectfile.csproj"), GenerationTargetLanguage.CSharp, "sampleCsProjectfile", "sampleCsProjectfile", "sampleCsProjectfile");
        }

        [Fact]
        public void Should_parse_CSProj_NewComplex_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly(PathHelper.SanitizeDirectorySeparatorChar(@"Data\CSProj_NewComplex\sampleCsProjectfile.csproj"), GenerationTargetLanguage.CSharp, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }

        [Fact]
        public void Should_parse_CSProj_NewWithExclude_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly(PathHelper.SanitizeDirectorySeparatorChar(@"Data\CSProj_NewWithExclude\sampleCsProjectfile.csproj"), GenerationTargetLanguage.CSharp, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }

        [Fact]
        public void Should_parse_ToolsVersion12_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly(PathHelper.SanitizeDirectorySeparatorChar(@"Data\CSProj_ToolsVersion_12\sampleCsProjectfile.csproj"), GenerationTargetLanguage.CSharp, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }

        [Fact]
        public void Should_parse_ToolsVersion12_vbproj_file_correctly()
        {
            Should_parse_csproj_file_correctly(PathHelper.SanitizeDirectorySeparatorChar(@"Data\VBProj_ToolsVersion_12\sampleCsProjectfile.vbproj"), GenerationTargetLanguage.VB, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }

        [Fact]
        public void Should_parse_ToolsVersion14_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly(PathHelper.SanitizeDirectorySeparatorChar(@"Data\CSProj_ToolsVersion_14\sampleCsProjectfile.csproj"), GenerationTargetLanguage.CSharp, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }

        [Fact]
        public void Should_parse_ToolsVersion14_vbproj_file_correctly()
        {
            Should_parse_csproj_file_correctly(PathHelper.SanitizeDirectorySeparatorChar(@"Data\VBProj_ToolsVersion_14\sampleCsProjectfile.vbproj"), GenerationTargetLanguage.VB, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }

        [Fact]
        public void Should_parse_ToolsVersion4_csproj_file_correctly()
        {
            Should_parse_csproj_file_correctly(PathHelper.SanitizeDirectorySeparatorChar(@"Data\CSProj_ToolsVersion_4\sampleCsProjectfile.csproj"), GenerationTargetLanguage.CSharp, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }

        [Fact]
        public void Should_parse_ToolsVersion4_vbproj_file_correctly()
        {
            Should_parse_csproj_file_correctly(PathHelper.SanitizeDirectorySeparatorChar(@"Data\VBProj_ToolsVersion_4\sampleCsProjectfile.vbproj"), GenerationTargetLanguage.VB, "Hacapp.Web.Tests.UI", "Hacapp.Web.Tests.UI", "sampleCsProjectfile");
        }
    }
}