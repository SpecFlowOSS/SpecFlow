using System;

#if NETSTANDARD
using System.Runtime.InteropServices;
#endif

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class SystemInformationProvider : ISystemInformationProvider
    {
#if NETSTANDARD
        public string GetCpuArchitecture() => RuntimeInformation.OSArchitecture.ToString();
        public string GetOperatingSystem() => RuntimeInformation.OSDescription;
#else
        public string GetCpuArchitecture() => Environment.Is64BitOperatingSystem ? "x86" : "x64";
        public string GetOperatingSystem() => "Windows";
#endif
    }
}
