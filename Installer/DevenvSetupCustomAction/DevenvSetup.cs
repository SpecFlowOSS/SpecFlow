using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;


namespace DevenvSetupCustomAction
{
    [RunInstaller(true)]
    public partial class DevenvSetup : Installer
    {
        public DevenvSetup()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            try
            {

                using (RegistryKey setupKey = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\VisualStudio\9.0\Setup\VS"))
                {
                    if (setupKey != null)
                    {
                        string devenv = setupKey.GetValue("EnvironmentPath").ToString();
                        if (!string.IsNullOrEmpty(devenv))
                        {
                            Process process = new Process();
                            process.StartInfo.FileName = Environment.ExpandEnvironmentVariables(devenv);
                            //process.StartInfo.Arguments = "/setup";
                            process.StartInfo.Arguments = "/installvstemplates";
                            process.Start();

                            process.WaitForExit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to execute 'devenv /setup'. Please perform this action manually." +
                                Environment.NewLine + ex.Message, "Installer Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }

        }
    }
}
