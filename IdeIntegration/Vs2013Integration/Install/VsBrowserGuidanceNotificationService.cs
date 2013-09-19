using System;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Install;
using Window = EnvDTE.Window;
using Timer = System.Windows.Forms.Timer;

namespace TechTalk.SpecFlow.Vs2010Integration.Install
{
    class VsBrowserGuidanceNotificationService : IGuidanceNotificationService
    {
        private readonly DTE dte;

        public VsBrowserGuidanceNotificationService(DTE dte)
        {
            this.dte = dte;
        }

        private Timer openPageTimer;
        private Timer activationTimer;

        public bool ShowPage(string url)
        {
            // we use winforms timers to avoid invoking UI actions from background threads, etc.
            openPageTimer = CreateTimer(() => OpenPage(url), 1);

            return true;
        }

        private Timer CreateTimer(Action action, int seconds)
        {
            var timer = new Timer();
            timer.Tick += (sender, args) => action();
            timer.Interval = seconds * 1000;
            timer.Start();
            return timer;
        }

        private void OpenPage(string url)
        {
            openPageTimer.Stop();
            openPageTimer = null;

            try
            {
                var window = dte.ItemOperations.Navigate(url, vsNavigateOptions.vsNavigateOptionsNewWindow);
                window.Activate();

                activationTimer = CreateTimer(() => ActivateWindow(window), 4);
            }
            catch (Exception)
            {
                // bad luck...
            }
        }

        private void ActivateWindow(Window window)
        {
            activationTimer.Stop();
            activationTimer = null;

            try
            {
                window.Activate();
            }
            catch (Exception)
            {
                // bad luck...
            }
        }
    }
}
