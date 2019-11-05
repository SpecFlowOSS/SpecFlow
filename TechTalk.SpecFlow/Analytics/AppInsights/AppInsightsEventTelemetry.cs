﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.Analytics.AppInsights
{
    /// <summary>
    /// For property names, check: https://github.com/microsoft/ApplicationInsights-Home/tree/master/EndpointSpecs/Schemas/Bond
    /// </summary>
    public class AppInsightsEventTelemetry
    {
        [DataMember(Name = "name")]
        public string DataTypeName { get; set; }

        [DataMember(Name = "time")]
        public string EventDateTime { get; set; }

        [DataMember(Name = "iKey")] 
        public string InstrumentationKey => AppInsightsInstrumentationKey.Key;

        [DataMember(Name = "data")]
        public TelemetryData TelemetryData { get; set; }

        public AppInsightsEventTelemetry(IAnalyticsEvent analyticsEvent)
        {
            DataTypeName = $"Microsoft.ApplicationInsights.{InstrumentationKey}.Event";

            EventDateTime = analyticsEvent.UtcDate.ToString("O");

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
                        { "SpecFlowVersion", analyticsEvent.SpecFlowVersion },
                        { "UnitTestProvider", analyticsEvent.UnitTestProvider },
                        { "IsBuildServer", analyticsEvent.IsBuildServer.ToString() },
                        { "HashedAssemblyName", analyticsEvent.HashedAssemblyName },
                        { "ProjectTargetFrameworks", analyticsEvent.ProjectTargetFrameworks },
                    }
                }
            };

            if (analyticsEvent is SpecFlowProjectCompilingEvent specFlowProjectCompiledEvent)
            {
                TelemetryData.TelemetryDataItem.Properties.Add("MSBuildVersion", specFlowProjectCompiledEvent.MSBuildVersion);
                TelemetryData.TelemetryDataItem.Properties.Add("ProjectGuid", specFlowProjectCompiledEvent.ProjectGuid);
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
