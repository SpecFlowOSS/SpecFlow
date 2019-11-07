using System;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Analytics.AppInsights;

namespace TechTalk.SpecFlow.Analytics
{
    public class HttpClientAnalyticsTransmitterSink : IAnalyticsTransmitterSink
    {
        private readonly IAppInsightsEventSerializer _appInsightsEventSerializer;
        private readonly Uri _appInsightsDataCollectionEndPoint = new Uri("https://dc.services.visualstudio.com/v2/track");

        public HttpClientAnalyticsTransmitterSink(IAppInsightsEventSerializer appInsightsEventSerializer)
        {
            _appInsightsEventSerializer = appInsightsEventSerializer;
        }

        public async Task TransmitEvent(IAnalyticsEvent analyticsEvent)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var serializedEventTelemetry = _appInsightsEventSerializer.SerializeAnalyticsEvent(analyticsEvent);

                    using (var httpContent = new ByteArrayContent(serializedEventTelemetry))
                    {
                        await httpClient.PostAsync(_appInsightsDataCollectionEndPoint, httpContent);
                    }
                }
            }
            catch (Exception)
            {
                //we don't care if we get any error during the sending
            }
        }
    }
}