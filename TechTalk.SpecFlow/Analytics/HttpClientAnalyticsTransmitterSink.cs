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
        private readonly HttpClientWrapper _httpClientWrapper;
        private Uri _appInsightsDataCollectionEndPoint;

        private Uri EndPoint => _appInsightsDataCollectionEndPoint ??= new Uri("https://dc.services.visualstudio.com/v2/track");

        public HttpClientAnalyticsTransmitterSink(IAppInsightsEventSerializer appInsightsEventSerializer, HttpClientWrapper httpClientWrapper)
        {
            _appInsightsEventSerializer = appInsightsEventSerializer;
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<IResult> TransmitEventAsync(IAnalyticsEvent analyticsEvent, string instrumentationKey)
        {
            try
            {
                await TransmitEventInternalAsync(analyticsEvent, instrumentationKey);
                return Result.Success();
            }
            catch (Exception e)
            {
                // we swallow the exception to not break the build when just the analytics could not be transmitted
                // but we still return it inside a Failure to tell the outside world that something has failed
                return Result.Failure(e);
            }
        }

        private async Task TransmitEventInternalAsync(IAnalyticsEvent analyticsEvent, string instrumentationKey)
        {
            var serializedEventTelemetry = _appInsightsEventSerializer.SerializeAnalyticsEvent(analyticsEvent, instrumentationKey);

            using (var httpContent = new ByteArrayContent(serializedEventTelemetry))
            {
                await _httpClientWrapper.HttpClient.PostAsync(EndPoint, httpContent);
            }
        }
    }
}