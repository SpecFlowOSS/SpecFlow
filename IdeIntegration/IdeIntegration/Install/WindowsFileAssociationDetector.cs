using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public class WindowsFileAssociationDetector : IFileAssociationDetector
    {
// ReSharper disable UnusedMember.Local
        [Flags]
        enum AssocF
        {
            Init_NoRemapCLSID = 0x1,
            Init_ByExeName = 0x2,
            Open_ByExeName = 0x2,
            Init_DefaultToStar = 0x4,
            Init_DefaultToFolder = 0x8,
            NoUserSettings = 0x10,
            NoTruncate = 0x20,
            Verify = 0x40,
            RemapRunDll = 0x80,
            NoFixUps = 0x100,
            IgnoreBaseClass = 0x200
        }

        enum AssocStr
        {
            Command = 1,
            Executable,
            FriendlyDocName,
            FriendlyAppName,
            NoOpen,
            ShellNewValue,
            DDECommand,
            DDEIfExec,
            DDEApplication,
            DDETopic,
            InfoTip,
            QuickTip,
            TileInfo,
            ContentType,
            DefaultIcon,
            ShellExtension
        }
// ReSharper restore UnusedMember.Local

        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint AssocQueryString(AssocF flags, AssocStr str, string pszAssoc, string pszExtra,
            [Out] StringBuilder pszOut, [In][Out] ref uint pcchOut);

        private readonly IIdeTracer tracer;

        public WindowsFileAssociationDetector(IIdeTracer tracer)
        {
            this.tracer = tracer;
        }

        public bool? IsAssociated()
        {
            try
            {
                const string extension = ".feature";

                uint resultLength = 0;
                const AssocStr assocStr = AssocStr.FriendlyDocName;
                AssocQueryString(AssocF.NoTruncate, assocStr, extension, null, null, ref resultLength);

                if (resultLength == 0)
                    return false;

                // Allocate the output buffer
                StringBuilder pszOut = new StringBuilder((int)resultLength);

                // Get the full pathname to the program in pszOut
                AssocQueryString(AssocF.Verify, assocStr, extension, null, pszOut, ref resultLength);

                string doc = pszOut.ToString();

                return doc.IndexOf("SpecFlow", StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
                tracer.Trace("IsAssociated falied: {0}", this, ex);
                return null;
            }
        }

        private bool EnsureIcon(string iconPath)
        {
            if (!File.Exists(iconPath))
            {
                var iconResource = this.GetType().Assembly.GetManifestResourceStream("TechTalk.SpecFlow.IdeIntegration.gherkin.ico");
                if (iconResource == null)
                    return false;

                using (iconResource)
                {

                    string folder = Path.GetDirectoryName(iconPath);
                    Debug.Assert(folder != null);
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    using (var outFile = new FileStream(iconPath, FileMode.Create))
                    {
                        byte[] iconBytes = new byte[iconResource.Length];
                        iconResource.Read(iconBytes, 0, iconBytes.Length);
                        outFile.Write(iconBytes, 0, iconBytes.Length);
                    }
                }
            }

            return true;
        }

        public bool SetAssociation()
        {
            try
            {
                const string progId = "SpecFlow.GherkinFile";
                const string extension = ".feature";
                const string friendlyTypeName = "Gherkin Specification File for SpecFlow";

                string appPath = Process.GetCurrentProcess().MainModule.FileName;
                string iconPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"SpecFlow\gherkin.ico");
                const string classesBaseKey = @"Software\Classes\" + progId;
                using (var key = Registry.CurrentUser.CreateSubKey(classesBaseKey, RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    if (key == null)
                        return false;

                    key.SetValue(null, friendlyTypeName);
                    key.SetValue("FriendlyTypeName", friendlyTypeName);
                }

                if (EnsureIcon(iconPath))
                {
                    using (var key = Registry.CurrentUser.CreateSubKey(classesBaseKey + @"\DefaultIcon", RegistryKeyPermissionCheck.ReadWriteSubTree)) 
                    {
                        if (key == null)
                            return false;

                        key.SetValue(null, iconPath);
                    }
                }

                using (var key = Registry.CurrentUser.CreateSubKey(classesBaseKey + @"\shell", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    if (key == null)
                        return false;

                    key.SetValue(null, "Open");
                }

                using (var key = Registry.CurrentUser.CreateSubKey(classesBaseKey + @"\shell\open", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    if (key == null)
                        return false;

                    key.SetValue(null, "&Open");
                }

                using (var key = Registry.CurrentUser.CreateSubKey(classesBaseKey + @"\shell\open\command", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    if (key == null)
                        return false;

                    key.SetValue(null, string.Format(@"""{0}"" /edit ""%1""", appPath));
                }

                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + extension, RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    if (key == null)
                        return false;

                    key.SetValue(null, progId);
                    key.SetValue("Content Type", "application/text");
                }
                return true;
            }
            catch (Exception ex)
            {
                tracer.Trace("SetAssociation failed: {0}", this, ex);
                return false;
            }
        }
    }
}
