﻿using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Analytics
{
    public abstract class SpecFlowAnalyticsEventBase : IAnalyticsEvent
    {
        public abstract string EventName { get; }
        public DateTime UtcDate { get; }
        public string UserId { get; }
        public string Platform { get; }
        public string SpecFlowVersion { get; }
        public string UnitTestProvider { get; }
        public bool IsBuildServer { get; }
        public string HashedAssemblyName { get; }
        public string ProjectTargetFrameworks { get; }

        protected SpecFlowAnalyticsEventBase(DateTime utcDate, string userId, string platform, string specFlowVersion, string unitTestProvider, bool isBuildServer, string hashedAssemblyName, string projectTargetFrameworks)
        {
            UtcDate = utcDate;
            UserId = userId;
            Platform = platform;
            SpecFlowVersion = specFlowVersion;
            UnitTestProvider = unitTestProvider;
            IsBuildServer = isBuildServer;
            HashedAssemblyName = hashedAssemblyName;
            ProjectTargetFrameworks = projectTargetFrameworks;
        }
    }
}
