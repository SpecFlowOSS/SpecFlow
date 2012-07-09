using System;
using System.Windows.Forms;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public interface IGuidanceNotificationService
    {
        bool ShowPage(string url);
    }

    class TestGuidanceNotificationService : IGuidanceNotificationService
    {
        public bool ShowPage(string url)
        {
            MessageBox.Show("TODO: " + url);

            return true;
        }
    }
}