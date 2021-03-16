using System;
using System.Text;
using TechTalk.SpecFlow.Analytics;
using TechTalk.SpecFlow.Analytics.AppInsights;
using TechTalk.SpecFlow.TinyJson;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Analytics
{
    public class AppInsightsEventTelemetrySerializerTests
    {
        [Fact]
        public void AppInsightsEventSerializationResultsTheSameString()
        {
            var eventTelemetry = CreateSampleAppInsinghtsEvent();

            var utf8JsonResult = Utf8Json.JsonSerializer.ToJsonString(eventTelemetry);
            var tinyJsonResult = eventTelemetry.ToJson();

            Assert.Equal(utf8JsonResult, tinyJsonResult);
        }

        [Fact]
        public void AppInsightsEventSerializationResultsTheSameByteArray()
        {
            var eventTelemetry = CreateSampleAppInsinghtsEvent();

            var utf8JsonResult = Utf8Json.JsonSerializer.Serialize(eventTelemetry);
            var tinyJsonResult = Encoding.UTF8.GetBytes(eventTelemetry.ToJson());

            Assert.Equal(utf8JsonResult, tinyJsonResult);
        }

        private AppInsightsEventTelemetry CreateSampleAppInsinghtsEvent()
        {
            var analyticsEvent = new SpecFlowProjectCompilingEvent(DateTime.Now, Guid.NewGuid().ToString(),
                                                                   "Windows", "Microsoft Windows 10.0.19041", "3.7.13+7faab2b5c0",
                                                                   "xunit", "", "852c8c2dbf14a96a81045593b3ff011d",
                                                                   "netcoreapp3.1;net5.0", "netcoreapp3.1", "16.6.0", "", false);

            return new AppInsightsEventTelemetry(analyticsEvent, Guid.NewGuid().ToString());
        }
    }
}
