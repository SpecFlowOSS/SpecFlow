using System;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public class SpecFlowInstallationStatus
    {
        public bool IsInstalled { get { return InstalledVersion != null; } }
        public Version InstalledVersion { get; set; }
        public DateTime? InstallDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public int UsageDays { get; set; }
        public int UserLevel { get; set; }
    }
}