using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using TechTalk.SpecFlow.Specs.MSBuild.Support;
using TechTalk.SpecFlow.Specs.Support;

namespace TechTalk.SpecFlow.Specs.MSBuild.Drivers
{
    [Binding]
    public class ProjectDriver : IDisposable
    {
        private string _projectName;

        private DirectoryInfo _projectLocation;

        private string _projectFile;

        private static readonly XNamespace Namespace =
#if NETCOREAPP2_2
                XNamespace.None;
#else
                XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");
#endif

        private readonly Logger _logger;

        private readonly ScenarioContext _scenarioContext;

        public ProjectDriver(Logger logger, ScenarioContext scenarioContext)
        {
            _logger = logger;
            _scenarioContext = scenarioContext;
            _logger.Verbosity = LoggerVerbosity.Minimal;
        }

        public void CreateProject()
        {
            _projectName = "TestProject";
        }

        public bool BuiltSuccessfully { get; private set; }

        public void BuildProject()
        {
            var properties = new Dictionary<string, string>();

            var projects = new ProjectCollection(properties);

            // Load the project file into the MSBuild system.
            var project = projects.LoadProject(_projectFile);

            BuiltSuccessfully = project.Build(_logger);
        }

        [BeforeScenarioBlock]
        public void BeforeScenarioBlock()
        {
            if (_scenarioContext.CurrentScenarioBlock == ScenarioBlock.Given)
            {
                MaterializeProject();
            }
        }

        private void MaterializeProject()
        {
            // Create a directory to house the project.
            _projectLocation = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "SpecFlow.GeneratedProject_" + Guid.NewGuid()));

            // Generate and write out the test project file.
            _projectFile = WriteProjectFile();
        }

        private string WriteProjectFile()
        {
            const string runtime =
#if NETCOREAPP2_2
                "netcoreapp2.0";
#else
                "net472";
#endif

            var builder = new ProjectFileBuilder
            {
                RepoRoot = Path.GetFullPath(@"..\..\..\..\..\"),

                AssemblyName = "TechTalk.Specflow.Specs.MSBuild.GenerationCandidate",
                RootNamespace = "TechTalk.Specflow.Specs.MSBuild.GenerationCandidate"
            };

#if !NETCOREAPP2_2

            // Add reference to common props.
            builder.AddPropFileImport(
                @"$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props",
                @"Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')");
#endif
            // Add reference to generator under test.
            builder.AddPropFileImport(@"$(RepoRoot)SpecFlow.Tools.MsBuild.Generation\build\SpecFlow.Tools.MsBuild.Generation.props");
            builder.AddTargetFileImport(@"$(RepoRoot)SpecFlow.Tools.MsBuild.Generation\build\SpecFlow.Tools.MsBuild.Generation.targets");
            builder.AddPropertyValue(
                "SpecFlowTasksAssembly",
                $@"$(RepoRoot)SpecFlow.Tools.MsBuild.Generation\bin\$(Configuration)\{runtime}\SpecFlow.Tools.MsBuild.Generation.dll");

            // Add references to XUnit plugin under test.
            builder.AddPropFileImport(@"$(RepoRoot)Plugins\TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin\build\SpecFlow.xUnit.props");
            builder.AddTargetFileImport(@"$(RepoRoot)Plugins\TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin\build\SpecFlow.xUnit.targets");
            builder.AddPropertyValue(
                "SpecFlowXUnitGeneratorPluginAssembly",
                $@"$(RepoRoot)Plugins\TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin\bin\$(Configuration)\{runtime}\TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin.dll");
            builder.AddPropertyValue(
                "SpecFlowXUnitRuntimePluginAssembly",
                $@"$(RepoRoot)Plugins\TechTalk.SpecFlow.xUnit.SpecFlowPlugin\bin\$(Configuration)\{runtime}\TechTalk.SpecFlow.xUnit.SpecFlowPlugin.dll");

#if !NETCOREAPP2_2

            // Add reference to CSharp targets.
            builder.AddTargetFileImport(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");
#endif

            // Add project reference to SpecFlow library.
#if !NETCOREAPP2_2
            builder.AddProjectReference(
                @"$(RepoRoot)TechTalk.SpecFlow\TechTalk.SpecFlow.csproj",
                additionalProperties: "TargetFramework=net472");
#else
            builder.AddProjectReference(
                @"$(RepoRoot)TechTalk.SpecFlow\TechTalk.SpecFlow.csproj");
#endif

            // Add package references for XUnit.
            builder.AddPackageReference("xunit", "2.4.1");
            builder.AddPackageReference(
                "xunit.runner.visualstudio",
                "2.4.1",
                privateAssets: "all",
                includeAssets: "runtime; build; native; contentfiles; analyzers");

            var projectFile = Path.Combine(_projectLocation.FullName, "TechTalk.Specflow.Specs.MSBuild.GenerationCandidate.csproj");

            builder.WriteToFile(projectFile);

            return projectFile;
        }

        public void AddFeatureFile(string content)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (_projectLocation != null)
            {
                Directory.Delete(_projectLocation.FullName, true);
            }
        }
    }
}
