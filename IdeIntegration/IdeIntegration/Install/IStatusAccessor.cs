using System;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public interface IStatusAccessor
    {
        bool UpdateStatus(SpecFlowInstallationStatus status);
        SpecFlowInstallationStatus GetInstallStatus();
    }
}