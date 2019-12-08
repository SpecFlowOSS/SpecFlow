using System.Runtime.InteropServices;

namespace TechTalk.SpecFlow.Specs.Drivers
{

    public class RuntimeInformationProvider
    {
#if !NETFRAMEWORK
        public bool IsOperatingSystemUnixoid() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public bool IsOperatingSystemWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
        public bool IsOperatingSystemUnixoid() => false;

        public bool IsOperatingSystemWindows() => true;
#endif
    }
}
