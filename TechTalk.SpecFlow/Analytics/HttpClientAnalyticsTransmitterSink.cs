using System;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Analytics.AppInsights;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.Analytics
{
    public class HttpClientAnalyticsTransmitterSink : IAnalyticsTransmitterSink
    {
        private readonly IAppInsightsEventSerializer _appInsightsEventSerializer;
        private readonly Uri _appInsightsDataCollectionEndPoint = new Uri("https://dc.services.visualstudio.com/v2/track");
        private readonly HttpClient _httpClient;

        public HttpClientAnalyticsTransmitterSink(IAppInsightsEventSerializer appInsightsEventSerializer, HttpClientWrapper httpClientWrapper)
        {
            _appInsightsEventSerializer = appInsightsEventSerializer;
            _httpClient = httpClientWrapper.HttpClient;
        }

        public async Task<IResult> TransmitEvent(IAnalyticsEvent analyticsEvent, string instrumentationKey)
        {
            try
            {
                await TransmitEventAsync(analyticsEvent, instrumentationKey);
                return Result.Success();
            }
            catch (Exception e)
            {
                // we swallow the exception to not break the build when just the analytics could not be transmitted
                // but we still return it inside a Failure to tell the outside world that something has failed
                return Result.Failure(e);
            }
        }

        public async Task TransmitEventAsync(IAnalyticsEvent analyticsEvent, string instrumentationKey)
        {
            var serializedEventTelemetry = _appInsightsEventSerializer.SerializeAnalyticsEvent(analyticsEvent, instrumentationKey);

            using (var httpContent = new ByteArrayContent(serializedEventTelemetry))
            {
                await _httpClient.PostAsync(_appInsightsDataCollectionEndPoint, httpContent);
            }
        }
    }
}