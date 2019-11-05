namespace TechTalk.SpecFlow.Analytics
{
    public interface IMsBuildTask
    {
        string Platform { get; set; }
        string BuildServerMode { get; set; }
        string MSBuildVersion { get; set; }
        string AssemblyName { get; set; }
        string TargetFrameworks { get; set; }
        string TargetFrameworkMoniker { get; set; }
        string ProjectGuid { get; set; }
    }
}
