using System;
using Microsoft.Win32;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public class RegistryStatusAccessor : IStatusAccessor
    {
        private const string REG_PATH = @"Software\TechTalk\SpecFlow";

        private readonly IIdeTracer tracer;

        public RegistryStatusAccessor(IIdeTracer tracer)
        {
            this.tracer = tracer;
        }

        private static string RegPath
        {
            get
            {
                var regPath = REG_PATH;
                if (InstallServices.IsDevBuild)
                    regPath += "Dev";
                return regPath;
            }
        }

        public SpecFlowInstallationStatus GetInstallStatus()
        {
            var status = new SpecFlowInstallationStatus();

            using (var key = Registry.CurrentUser.OpenSubKey(RegPath, RegistryKeyPermissionCheck.ReadSubTree))
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

        public bool UpdateStatus(SpecFlowInstallationStatus status)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(RegPath, RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                if (key == null)
                    return false;

                if (status.InstalledVersion != null)
                    key.SetValue("version", status.InstalledVersion);
                if (status.InstallDate != null)
                    key.SetValue("installDate", SerializeDate(status.InstallDate));
                if (status.LastUsedDate != null)
                    key.SetValue("lastUsedDate", SerializeDate(status.LastUsedDate));
                key.SetValue("usageDays", status.UsageDays);
                key.SetValue("userLevel", status.UserLevel);
            }
            return true;
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