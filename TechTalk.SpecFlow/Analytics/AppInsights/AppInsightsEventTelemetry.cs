using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.Analytics.AppInsights
{
    /// <summary>
    /// For property names, check: https://github.com/microsoft/ApplicationInsights-Home/tree/master/EndpointSpecs/Schemas/Bond
    /// For tags, check: https://github.com/microsoft/ApplicationInsights-Home/blob/master/EndpointSpecs/Schemas/Bond/ContextTagKeys.bond
    /// </summary>
    public class AppInsightsEventTelemetry
    {
        [DataMember(Name = "name")]
        public string DataTypeName { get; set; }

        [DataMember(Name = "time")]
        public string EventDateTime { get; set; }

        [DataMember(Name = "iKey")] 
        public string InstrumentationKey { get; set; }

        [DataMember(Name = "data")]
        public TelemetryData TelemetryData { get; set; }

        [DataMember(Name = "tags")]
        public Dictionary<string, string> TelemetryTags { get; set; }

        private const string DefaultValue = "undefined";

        public AppInsightsEventTelemetry(IAnalyticsEvent analyticsEvent, string instrumentationKey)
        {
            InstrumentationKey = instrumentationKey;
            DataTypeName = $"Microsoft.ApplicationInsights.{InstrumentationKey}.Event";

            EventDateTime = analyticsEvent.UtcDate.ToString("O");

            TelemetryTags = new Dictionary<string, string>()
            {
                { "ai.user.id", analyticsEvent.UserId },
                { "ai.user.accountId", analyticsEvent.UserId }
            };

            TelemetryData = new TelemetryData
            {
                ItemTypeName = "EventData",
                TelemetryDataItem = new TelemetryDataItem
                {
                    EventName = analyticsEvent.EventName,
                    Properties = new Dictionary<string, string>()
                    {
                        { "UtcDate", analyticsEvent.UtcDate.ToString("O") },
                        { "UserId", analyticsEvent.UserId },
                        { "Platform", analyticsEvent.Platform },
                        { "PlatformDescription", analyticsEvent.PlatformDescription },
                        { "SpecFlowVersion", analyticsEvent.SpecFlowVersion },
                        { "UnitTestProvider", analyticsEvent.UnitTestProvider ?? DefaultValue },
                        { "IsBuildServer", analyticsEvent.IsBuildServer.ToString() },
                        { "BuildServerName", analyticsEvent.BuildServerName ?? DefaultValue },
                        { "IsDockerContainer", analyticsEvent.IsDockerContainer.ToString() },
                        { "HashedAssemblyName", analyticsEvent.HashedAssemblyName ?? DefaultValue },
                        { "TargetFrameworks", analyticsEvent.TargetFrameworks },
                        { "TargetFramework", analyticsEvent.TargetFramework },
                    }
                }
            };

            if (analyticsEvent is SpecFlowProjectCompilingEvent specFlowProjectCompiledEvent)
            {
                TelemetryData.TelemetryDataItem.Properties.Add("MSBuildVersion", specFlowProjectCompiledEvent.MSBuildVersion);
                TelemetryData.TelemetryDataItem.Properties.Add("ProjectGuid", specFlowProjectCompiledEvent.ProjectGuid ?? DefaultValue);
            }
        }
    }
    public class TelemetryData
    {
        [DataMember(Name = "baseType")]
        public string ItemTypeName { get; set; }
        
        [DataMember(Name = "baseData")]
        public TelemetryDataItem TelemetryDataItem { get; set; }
    }

    public class TelemetryDataItem
    {
        [DataMember(Name = "ver")]
        public string EndPointSchemaVersion => "2";
        [DataMember(Name = "name")]
        public string EventName { get; set; }
        [DataMember(Name = "properties")]
        public Dictionary<string, string> Properties { get; set; }
    }
}
