using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Analytics.AppInsights;

namespace TechTalk.SpecFlow.Analytics
{
    public class HttpClientAnalyticsTransmitterSink : IAnalyticsTransmitterSink
    {
        private readonly IAppInsightsEventSerializer _appInsightsEventSerializer;
        public const string AppInsightsDataCollectionEndPoint = "https://dc.services.visualstudio.com/v2/track";

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
                    var requestUri = new Uri(AppInsightsDataCollectionEndPoint);
                    
                    var serializedEventTelemetry = _appInsightsEventSerializer.SerializeAnalyticsEvent(analyticsEvent);
                    
                    using (var httpContent = new ByteArrayContent(serializedEventTelemetry))
                    {
                        await httpClient.PostAsync(requestUri, httpContent);
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