using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.Support
{
    [Binding]
    public class StepArgumentTransformations
    {

        [StepArgumentTransformation(@"enabled")]
        public bool ConvertEnabled() { return true; }

        [StepArgumentTransformation(@"disabled")]
        public bool ConvertDisabled() { return false; }

        [StepArgumentTransformation("dotnet msbuild")]
        public BuildTool ConvertDotnetMSBuildToBuildTool() => BuildTool.DotnetMSBuild;

        [StepArgumentTransformation("dotnet build")]
        public BuildTool ConvertDotnetBuildToBuildTool() => BuildTool.DotnetBuild;

        [StepArgumentTransformation("MSBuild")]
        public BuildTool ConvertMSBuildToBuildTool() => BuildTool.MSBuild;
    }
}
