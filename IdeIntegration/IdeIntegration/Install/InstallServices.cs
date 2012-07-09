using System;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

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

    public enum GuidanceNotification
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

        private readonly IIdeTracer tracer;
        private readonly IGuidanceNotificationService notificationService;
        private readonly string regPath;

        public IdeIntegration IdeIntegration { get; private set; }
        public Version CurrentVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        public bool IsDevBuild
        {
            get { return CurrentVersion.Equals(new Version(1, 0)); }
        }

        public InstallServices(IGuidanceNotificationService notificationService, IIdeTracer tracer)
        {
            this.notificationService = notificationService;
            this.tracer = tracer;
            IdeIntegration = IdeIntegration.Unknown;

            regPath = REG_PATH;
            if (IsDevBuild)
                regPath += "Dev";
        }

        public void OnPackageLoad(IdeIntegration ideIntegration)
        {
            IdeIntegration = ideIntegration;

            if (IsDevBuild)
                tracer.Trace("Running on 'dev' version", this);

            var today = DateTime.Today;
            var status = GetInstallStatus();

            if (!status.IsInstalled)
            {
                // new user
                if (ShowNotification(GuidanceNotification.AfterInstall))
                {
                    status.InstallDate = today;
                    status.InstalledVersion = CurrentVersion;
                    status.LastUsedDate = today;

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
            int linkid = (int)guidanceNotification + (int)IdeIntegration;
            string url = string.Format("http://go.specflow.org/g{0}", linkid);

            return notificationService.ShowPage(url);
        }

        private const string REG_PATH = @"Software\TechTalk\SpecFlow";

        private void UpdateStatus(SpecFlowInstallationStatus status)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(REG_PATH, RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                if (key == null)
                    return;

                key.SetValue("version", status.InstalledVersion);
                key.SetValue("installDate", SerializeDate(status.InstallDate));
                key.SetValue("lastUsedDate", SerializeDate(status.LastUsedDate));
                key.SetValue("usageDays", status.UsageDays);
                key.SetValue("userLevel", status.UserLevel);
            }
        }

        private SpecFlowInstallationStatus GetInstallStatus()
        {
            var status = new SpecFlowInstallationStatus();

            using (var key = Registry.CurrentUser.OpenSubKey(REG_PATH, RegistryKeyPermissionCheck.ReadSubTree))
            {
                if (key == null)
                    return status;

                status.InstalledVersion = ReadStringValue(key, "version", s => new Version(s));
                status.InstallDate = ReadIntValue(key, "installDate", DeserializeDate);
                status.LastUsedDate = ReadIntValue(key, "lastUsedDate", DeserializeDate);
                status.UsageDays = ReadIntValue(key, "usageDays", i => i);
                status.UserLevel = ReadIntValue(key, "userLevel", i => i);
            }

            return status;
        }

        private T ReadStringValue<T>(RegistryKey key, string name, Func<string, T> converter)
        {
            try
            {
                var value = key.GetValue(name) as string;
                if (string.IsNullOrEmpty(value))
                    return default(T);
                return converter(value);
            }
            catch(Exception ex)
            {
                tracer.Trace("Registry read error: {0}", this, ex);
                return default(T);
            }
        }

        private T ReadIntValue<T>(RegistryKey key, string name, Func<int, T> converter)
        {
            try
            {
                var value = key.GetValue(name);
                if (value == null || !(value is int))
                    return default(T);
                return converter((int)value);
            }
            catch(Exception ex)
            {
                tracer.Trace("Registry read error: {0}", this, ex);
                return default(T);
            }
        }

        private readonly DateTime magicDate = new DateTime(2009, 9, 11); //when SpecFlow has born
        private DateTime? DeserializeDate(int days)
        {
            if (days <= 0)
                return null;
            return magicDate.AddDays(days);
        }

        private int SerializeDate(DateTime? dateTime)
        {
            if (dateTime == null)
                return 0;

            return (int)dateTime.Value.Date.Subtract(magicDate).TotalDays;
        }

    }
}
