using System;
using System.Diagnostics;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public class ExternalBrowserGuidanceNotificationService : IGuidanceNotificationService
    {
        private readonly IIdeTracer tracer;

        public ExternalBrowserGuidanceNotificationService(IIdeTracer tracer)
        {
            this.tracer = tracer;
        }

        public bool ShowPage(string url)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                tracer.Trace("Showing page for {0} skipped, because the machine is offline", this, url);
                return false;
            }

            try
            {
                Process.Start(url);
                return true;
            }
            catch (Exception ex)
            {
                tracer.Trace("Browser start error: {0}", this, ex);
                return false;
            }
        }
    }
}