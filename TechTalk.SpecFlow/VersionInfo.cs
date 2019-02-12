namespace TechTalk.SpecFlow
{
	internal class VersionInfo
    {
        internal static string AssemblyVersion => ThisAssembly.AssemblyVersion;
        internal static string AssemblyFileVersion => ThisAssembly.AssemblyFileVersion;
        internal static string AssemblyInformationalVersion => ThisAssembly.AssemblyInformationalVersion;
        internal static string NuGetVersion => ThisAssembly.AssemblyInformationalVersion.Replace("+", "-");
    }
}
