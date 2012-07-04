using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    internal class SpecFlowInstallationStatus
    {
        public bool IsInstalled { get { return InstalledVersion != null; } }
        public Version InstalledVersion { get; set; }
        public DateTime? InstallDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public int UsageDays { get; set; }
        public int UserLevel { get; set; }
    }

    internal enum GuidanceNotification
    {
        AfterInstall = 1,
        Upgrade = 2,
        AfterRampUp = 100,
        Experienced = 200,
        Veteran = 300
    }

    public class InstallServices
    {
        private const int AFTER_RAMP_UP_DAYS = 10;
        private const int EXPERIENCED_DAYS = 100;
        private const int VETERAN_DAYS = 300;

        public IdeIntegration IdeIntegration { get; private set; }
        public Version CurrentVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
        public bool IsDevBuild { get { return CurrentVersion.Equals(new Version(1, 0)); } }

        public InstallServices()
        {
            IdeIntegration = IdeIntegration.Unknown;
        }

        public void OnPackageLoad(IdeIntegration ideIntegration)
        {
            IdeIntegration = ideIntegration;

            if (IsDevBuild)
                return;

            var today = DateTime.Today;
            var status = GetInstallStatus();

            if (!status.IsInstalled)
            {
                // new user
                if (ShowNotification(GuidanceNotification.AfterInstall))
                {
                    status.InstallDate = today;
                    status.InstalledVersion = CurrentVersion;

                    UpdateStatus(status);
                }
            }
            else if (status.InstalledVersion < CurrentVersion)
            {
                //upgrading user   
                if (ShowNotification(GuidanceNotification.Upgrade))
                {
                    status.InstallDate = today;
                    status.InstalledVersion = CurrentVersion;

                    UpdateStatus(status);
                }
            }
            
            if (status.LastUsedDate != today)
            {
                //a shiny new day with SpecFlow
                status.UsageDays++;
                status.LastUsedDate = today;
                UpdateStatus(status);
            }

            if (status.UsageDays >= AFTER_RAMP_UP_DAYS && status.UserLevel < (int)GuidanceNotification.AfterRampUp)
            {
                if (ShowNotification(GuidanceNotification.AfterRampUp))
                {
                    status.UserLevel = (int)GuidanceNotification.AfterRampUp;
                    UpdateStatus(status);
                }
            }
            else if (status.UsageDays >= EXPERIENCED_DAYS && status.UserLevel < (int)GuidanceNotification.Experienced)
            {
                if (ShowNotification(GuidanceNotification.Experienced))
                {
                    status.UserLevel = (int)GuidanceNotification.Experienced;
                    UpdateStatus(status);
                }
            }
            else if (status.UsageDays >= VETERAN_DAYS && status.UserLevel < (int)GuidanceNotification.Veteran)
            {
                if (ShowNotification(GuidanceNotification.Veteran))
                {
                    status.UserLevel = (int)GuidanceNotification.Veteran;
                    UpdateStatus(status);
                }
            }
        }   

        private bool ShowNotification(GuidanceNotification guidanceNotification)
        {
            throw new NotImplementedException();
        }

        private void UpdateStatus(SpecFlowInstallationStatus status)
        {
            throw new NotImplementedException();
        }

        private SpecFlowInstallationStatus GetInstallStatus()
        {
            throw new NotImplementedException();
        }
    }
}
