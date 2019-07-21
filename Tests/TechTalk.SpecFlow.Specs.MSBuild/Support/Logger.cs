using System;
using Microsoft.Build.Framework;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.Specs.Support
{
    public class Logger : ILogger
    {
        private readonly ISpecFlowOutputHelper _specFlowOutputHelper;

        public Logger(ISpecFlowOutputHelper specFlowOutputHelper)
        {
            _specFlowOutputHelper = specFlowOutputHelper;
        }

        public LoggerVerbosity Verbosity { get; set; }

        public string Parameters { get; set; }

        public void Initialize(IEventSource eventSource)
        {
            if (eventSource != null)
            {
                eventSource.BuildStarted += WhenBuildStarted;
                eventSource.BuildFinished += WhenBuildFinished;
                eventSource.ProjectStarted += WhenProjectStarted;
                eventSource.ProjectFinished += WhenProjectFinished;
                eventSource.TargetStarted += WhenTargetStarted;
                eventSource.TargetFinished += WhenTargetFinished;
                eventSource.TaskStarted += WhenTaskStarted;
                eventSource.TaskFinished += WhenTaskFinished;
                eventSource.ErrorRaised += WhenError;
                eventSource.WarningRaised += WhenWarning;
                eventSource.MessageRaised += WhenMessage;
                eventSource.CustomEventRaised += WhenCustomEvent;
                eventSource.StatusEventRaised += WhenStatusEvent;
            }
        }

        private void WhenStatusEvent(object sender, BuildStatusEventArgs e)
        {
        }

        private void WhenCustomEvent(object sender, CustomBuildEventArgs e)
        {
        }

        private void WhenMessage(object sender, BuildMessageEventArgs e)
        {
        }

        private void WhenWarning(object sender, BuildWarningEventArgs e)
        {
            _specFlowOutputHelper.WriteLine(e.Message);
        }

        private void WhenError(object sender, BuildErrorEventArgs e)
        {
            _specFlowOutputHelper.WriteLine(e.Message);
        }

        private void WhenTaskFinished(object sender, TaskFinishedEventArgs e)
        {
        }

        private void WhenTaskStarted(object sender, TaskStartedEventArgs e)
        {
        }

        private void WhenTargetFinished(object sender, TargetFinishedEventArgs e)
        {
        }

        private void WhenTargetStarted(object sender, TargetStartedEventArgs e)
        {
        }

        private void WhenProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            _specFlowOutputHelper.WriteLine(e.Message);
        }

        private void WhenProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            _specFlowOutputHelper.WriteLine(e.Message);
        }

        private void WhenBuildFinished(object sender, BuildFinishedEventArgs e)
        {
            _specFlowOutputHelper.WriteLine($"------ {e.Message} ------");
        }

        private void WhenBuildStarted(object sender, BuildStartedEventArgs e)
        {
            _specFlowOutputHelper.WriteLine($"------ {e.Message} ------");
        }

        public void Shutdown()
        {
        }

        public void WriteLine(string message)
        {
            _specFlowOutputHelper.WriteLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            _specFlowOutputHelper.WriteLine(format, args);
        }
    }
}