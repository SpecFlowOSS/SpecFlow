using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Install;
using Window = EnvDTE.Window;

namespace TechTalk.SpecFlow.Vs2010Integration.Install
{
    class VsBrowserGuidanceNotificationService : IGuidanceNotificationService
    {
        private readonly DTE dte;

        public VsBrowserGuidanceNotificationService(DTE dte)
        {
            this.dte = dte;
        }

        private System.Windows.Forms.Timer activationTimer;

        public bool ShowPage(string url)
        {
            var window = dte.ItemOperations.Navigate(url, vsNavigateOptions.vsNavigateOptionsNewWindow);

            activationTimer = new System.Windows.Forms.Timer();
            activationTimer.Tick += (sender, args) => ActivateWindow(window);
            activationTimer.Interval = 5000;
            activationTimer.Start();

            return true;
        }

        private void ActivateWindow(object state)
        {
            activationTimer.Stop();

            try
            {
                Window window = (Window)state;
                window.Activate();
            }
            catch (Exception)
            {
                // bad luck...
            }
        }
    }
}
