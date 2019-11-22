using System;
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

        public MSBuildTaskAnalyticsTransmitter(
            IAnalyticsEventProvider analyticsEventProvider,
            IMSBuildInformationProvider msBuildInformationProvider,
            SpecFlowProjectInfo specFlowProjectInfo,
            IAnalyticsTransmitter analyticsTransmitter)
        {
            _analyticsEventProvider = analyticsEventProvider;
            _msBuildInformationProvider = msBuildInformationProvider;
            _specFlowProjectInfo = specFlowProjectInfo;
            _analyticsTransmitter = analyticsTransmitter;
        }

        public IResult TryTransmitProjectCompilingEvent()
        {
            try
            {
                return TransmitProjectCompilingEvent();
            }
            catch (Exception exc)
            {
                // catch all exceptions since we do not want to break the build simply because event creation failed
                // but still return an error containing the exception to at least log it
                return Result.Failure(exc);
            }
        }

        public IResult TransmitProjectCompilingEvent()
        {
            var projectCompilingEvent = _analyticsEventProvider.CreateProjectCompilingEvent(
                _msBuildInformationProvider.GetMSBuildVersion(),
                _specFlowProjectInfo.ProjectAssemblyName,
                _specFlowProjectInfo.TargetFrameworks,
                _specFlowProjectInfo.CurrentTargetFramework,
                _specFlowProjectInfo.ProjectGuid);
            return _analyticsTransmitter.TransmitSpecFlowProjectCompilingEvent(projectCompilingEvent);
        }
    }
}
