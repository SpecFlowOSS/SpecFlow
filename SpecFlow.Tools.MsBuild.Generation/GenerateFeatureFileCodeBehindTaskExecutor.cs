using System;
using System.Collections.Generic;
using BoDi;
using Microsoft.Build.Framework;
using TechTalk.SpecFlow.CommonModels;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class GenerateFeatureFileCodeBehindTaskExecutor : IGenerateFeatureFileCodeBehindTaskExecutor
    {
        private readonly IProcessInfoDumper _processInfoDumper;
        private readonly ITaskLoggingWrapper _taskLoggingWrapper;
        private readonly ISpecFlowProjectProvider _specFlowProjectProvider;
        private readonly SpecFlowProjectInfo _specFlowProjectInfo;
        private readonly WrappedGeneratorContainerBuilder _wrappedGeneratorContainerBuilder;
        private readonly IObjectContainer _rootObjectContainer;
        private readonly IMSBuildTaskAnalyticsTransmitter _msbuildTaskAnalyticsTransmitter;
        private readonly IExceptionTaskLogger _exceptionTaskLogger;

        public GenerateFeatureFileCodeBehindTaskExecutor(
            IProcessInfoDumper processInfoDumper,
            ITaskLoggingWrapper taskLoggingWrapper,
            ISpecFlowProjectProvider specFlowProjectProvider,
            SpecFlowProjectInfo specFlowProjectInfo,
            WrappedGeneratorContainerBuilder wrappedGeneratorContainerBuilder,
            IObjectContainer rootObjectContainer,
            IMSBuildTaskAnalyticsTransmitter msbuildTaskAnalyticsTransmitter,
            IExceptionTaskLogger exceptionTaskLogger)
        {
            _processInfoDumper = processInfoDumper;
            _taskLoggingWrapper = taskLoggingWrapper;
            _specFlowProjectProvider = specFlowProjectProvider;
            _specFlowProjectInfo = specFlowProjectInfo;
            _wrappedGeneratorContainerBuilder = wrappedGeneratorContainerBuilder;
            _rootObjectContainer = rootObjectContainer;
            _msbuildTaskAnalyticsTransmitter = msbuildTaskAnalyticsTransmitter;
            _exceptionTaskLogger = exceptionTaskLogger;
        }

        public IResult<IReadOnlyCollection<ITaskItem>> Execute()
        {
            _processInfoDumper.DumpProcessInfo();
            _taskLoggingWrapper.LogMessage("Starting GenerateFeatureFileCodeBehind");

            var specFlowProject = _specFlowProjectProvider.GetSpecFlowProject();

            using (var generatorContainer = _wrappedGeneratorContainerBuilder.BuildGeneratorContainer(
                specFlowProject.ProjectSettings.ConfigurationHolder,
                specFlowProject.ProjectSettings,
                _specFlowProjectInfo.GeneratorPlugins,
                _rootObjectContainer))
            {
                var projectCodeBehindGenerator = generatorContainer.Resolve<IProjectCodeBehindGenerator>();

                try
                {
                    var returnValue = projectCodeBehindGenerator.GenerateCodeBehindFilesForProject();

                    var transmissionResult = _msbuildTaskAnalyticsTransmitter.TryTransmitProjectCompilingEvent();

                    if (transmissionResult is IFailure failure)
                    {
                        _taskLoggingWrapper.LogMessageWithLowImportance($"Could not transmit analytics: {failure}");
                    }

                    if (_taskLoggingWrapper.HasLoggedErrors())
                    {
                        return Result<IReadOnlyCollection<ITaskItem>>.Failure("Feature file code-behind generation has failed with errors.");
                    }

                    return Result.Success(returnValue);
                }
                catch (Exception e)
                {
                    _exceptionTaskLogger.LogException(e);
                    return Result<IReadOnlyCollection<ITaskItem>>.Failure(e);
                }
            }
        }
    }
}