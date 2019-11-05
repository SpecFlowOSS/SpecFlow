using System;
using System.Reflection;
using System.Text;
using BoDi;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Analytics
{
    public class AnalyticsTransmitter : IAnalyticsTransmitter
    {
        private readonly IAnalyticsTransmitterSink _analyticsTransmitterSink;
        private readonly string _unitTestProvider;

        public AnalyticsTransmitter(IObjectContainer container)
        {
            _unitTestProvider = container.Resolve<UnitTestProviderConfiguration>().UnitTestProvider;
            _analyticsTransmitterSink = container.Resolve<IAnalyticsTransmitterSink>();
        }

        public void TransmitSpecflowProjectCompilingEvent(IMsBuildTask task)
        {
            try
            {
                //todo: check if telemetry is enabled
                var compilingEvent = CreateEvent(task);

                _analyticsTransmitterSink.TransmitEvent(compilingEvent);
            }
            catch (Exception)
            {
                //nope
            }
        }

        private SpecFlowProjectCompilingEvent CreateEvent(IMsBuildTask task)
        {
            //todo: use userId retrieving from VS Extension
            var userId = "getuserId";

            var unittestProvider = _unitTestProvider;
            var specFlowVersion = GetSpecFlowVersion();
            var isBuildServer = GetIsBuildServerMode(task.BuildServerMode);
            var hashedAssemblyName = ToSha256(task.AssemblyName);

            var compiledEvent = new SpecFlowProjectCompilingEvent(DateTime.UtcNow, userId,
                task.Platform, specFlowVersion, unittestProvider, isBuildServer,
                hashedAssemblyName, task.TargetFrameworks, task.MSBuildVersion, task.ProjectGuid);
            return compiledEvent;
        }

        private string GetSpecFlowVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version.ToString(2);
        }

        private bool GetIsBuildServerMode(string buildServerMode)
        {
            if (string.IsNullOrEmpty(buildServerMode))
            {
                return false;
            }
            return bool.Parse(buildServerMode);
        }

        private string ToSha256(string inputString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var stringBuilder = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            foreach (byte theByte in crypto)
            {
                stringBuilder.Append(theByte.ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }
}