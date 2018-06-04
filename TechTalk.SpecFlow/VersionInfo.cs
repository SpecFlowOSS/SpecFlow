using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow
{
    public class VersionInfo
    {
        public static string AssemblyVersion => ThisAssembly.AssemblyVersion;
        public static string AssemblyFileVersion => ThisAssembly.AssemblyFileVersion;
        public static string AssemblyInformationalVersion => ThisAssembly.AssemblyInformationalVersion;
        public static string NuGetVersion => ThisAssembly.AssemblyInformationalVersion.Replace("+", "-");
    }
}
