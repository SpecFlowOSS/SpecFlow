namespace TechTalk.SpecFlow.Analytics.AppInsights
{
    public interface IAppInsightsEventSerializer
    {
        byte[] SerializeAnalyticsEvent(IAnalyticsEvent analyticsEvent, string instrumentationKey);
    }
}
