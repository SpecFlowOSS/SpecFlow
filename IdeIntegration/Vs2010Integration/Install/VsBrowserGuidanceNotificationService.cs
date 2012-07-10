using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Install;

namespace TechTalk.SpecFlow.Vs2010Integration.Install
{
    class VsBrowserGuidanceNotificationService : IGuidanceNotificationService
    {
        private readonly DTE dte;

        public VsBrowserGuidanceNotificationService(DTE dte)
        {
            this.dte = dte;
        }

        public bool ShowPage(string url)
        {
            var window = dte.ItemOperations.Navigate(url, vsNavigateOptions.vsNavigateOptionsNewWindow);
            return true;
        }
    }
}
