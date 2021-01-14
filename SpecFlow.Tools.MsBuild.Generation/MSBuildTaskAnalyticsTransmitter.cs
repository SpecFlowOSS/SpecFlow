using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Analytics;
using TechTalk.SpecFlow.CommonModels;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class MSBuildTaskAnalyticsTransmitter : IMSBuildTaskAnalyticsTransmitter
    {
        private readonly IAnalyticsEventProvider _analyticsEventProvider;
        private readonly IMSBuildInformationProvider _msBuildInformationProvider;
        private readonly SpecFlowProjectInfo _specFlowProjectInfo;
        private readonly IAnalyticsTransmitter _analyticsTransmitter;
        private readonly ITaskLoggingWrapper _taskLoggingWrapper;

        public MSBuildTaskAnalyticsTransmitter(
            IAnalyticsEventProvider analyticsEventProvider,
            IMSBuildInformationProvider msBuildInformationProvider,
            SpecFlowProjectInfo specFlowProjectInfo,
            IAnalyticsTransmitter analyticsTransmitter,
            ITaskLoggingWrapper taskLoggingWrapper)
        {
            _analyticsEventProvider = analyticsEventProvider;
            _msBuildInformationProvider = msBuildInformationProvider;
            _specFlowProjectInfo = specFlowProjectInfo;
            _analyticsTransmitter = analyticsTransmitter;
            _taskLoggingWrapper = taskLoggingWrapper;
        }

        public async Task TryTransmitProjectCompilingEventAsync()
        {
            try
            {
                var transmissionResult = await TransmitProjectCompilingEventAsync();

                if (transmissionResult is IFailure failure)
                {
                    _taskLoggingWrapper.LogMessageWithLowImportance($"Could not transmit analytics: {failure}");
                }
            }
            catch (Exception exc)
            {
                // catch all exceptions since we do not want to break the build simply because event creation failed
                // but still return an error containing the exception to at least log it
                _taskLoggingWrapper.LogMessageWithLowImportance($"Could not transmit analytics: {exc}");
            }
        }

        public async Task<IResult> TransmitProjectCompilingEventAsync()
        {
            var projectCompilingEvent = _analyticsEventProvider.CreateProjectCompilingEvent(
                _msBuildInformationProvider.GetMSBuildVersion(),
                _specFlowProjectInfo.ProjectAssemblyName,
                _specFlowProjectInfo.TargetFrameworks,
                _specFlowProjectInfo.CurrentTargetFramework,
                _specFlowProjectInfo.ProjectGuid);
            return await _analyticsTransmitter.TransmitSpecFlowProjectCompilingEvent(projectCompilingEvent);
        }
    }
}
