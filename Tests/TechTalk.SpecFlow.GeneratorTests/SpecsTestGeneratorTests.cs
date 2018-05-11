using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Project;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public class SpecsTestGeneratorTests
    {
        [Fact]
        public void Test()
        {
            Environment.SetEnvironmentVariable("MSBuildToolsPath32", "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Enterprise\\MSBuild\\15.0\\Bin", EnvironmentVariableTarget.Process);

            var buildalyzerProjectReader = new BuildalyzerProjectReader(new GeneratorConfigurationProvider(new ConfigurationLoader()));

            var readSpecFlowProject = buildalyzerProjectReader.ReadSpecFlowProject("G:\\Work\\SpecFlow\\DefaultTestProject\\DefaultTestProject.csproj");

            readSpecFlowProject.FeatureFiles.Count.Should().Be(1);
        }
    }
}
