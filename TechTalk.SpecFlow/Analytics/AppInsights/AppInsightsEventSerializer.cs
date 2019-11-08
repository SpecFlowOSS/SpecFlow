namespace TechTalk.SpecFlow.Analytics.AppInsights
{
    public class AppInsightsEventSerializer : IAppInsightsEventSerializer
    {
        public byte[] SerializeAnalyticsEvent(IAnalyticsEvent analyticsEvent)
        {
            var eventTelemetry = new AppInsightsEventTelemetry(analyticsEvent);
            return Utf8Json.JsonSerializer.Serialize(eventTelemetry);
        }
    }
}