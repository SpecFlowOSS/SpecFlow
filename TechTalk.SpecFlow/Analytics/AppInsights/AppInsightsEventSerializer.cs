﻿using System.Text;
using TechTalk.SpecFlow.TinyJson;

namespace TechTalk.SpecFlow.Analytics.AppInsights
{
    public class AppInsightsEventSerializer : IAppInsightsEventSerializer
    {
        public byte[] SerializeAnalyticsEvent(IAnalyticsEvent analyticsEvent, string instrumentationKey)
        {
            var eventTelemetry = new AppInsightsEventTelemetry(analyticsEvent, instrumentationKey);
            return Encoding.UTF8.GetBytes(eventTelemetry.ToJson());
        }
    }
}