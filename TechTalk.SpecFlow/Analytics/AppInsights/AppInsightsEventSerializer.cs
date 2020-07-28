namespace TechTalk.SpecFlow.Analytics.AppInsights
{
    public class AppInsightsEventSerializer : IAppInsightsEventSerializer
    {
        public byte[] SerializeAnalyticsEvent(IAnalyticsEvent analyticsEvent, string instrumentationKey)
        {
            var eventTelemetry = new AppInsightsEventTelemetry(analyticsEvent, instrumentationKey);
            return Utf8Json.JsonSerializer.Serialize(eventTelemetry);
        }
    }
}