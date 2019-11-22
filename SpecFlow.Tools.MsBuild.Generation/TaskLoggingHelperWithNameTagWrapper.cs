using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class TaskLoggingHelperWithNameTagWrapper : ITaskLoggingWrapper
    {
        private readonly TaskLoggingHelper _taskLoggingHelper;

        public TaskLoggingHelperWithNameTagWrapper(TaskLoggingHelper taskLoggingHelper)
        {
            _taskLoggingHelper = taskLoggingHelper;
        }

        public void LogMessage(string message)
        {
            string messageWithNameTag = GetMessageWithNameTag(message);
            _taskLoggingHelper.LogMessage(messageWithNameTag);
        }

        public void LogMessageWithLowImportance(string message)
        {
            string messageWithNameTag = GetMessageWithNameTag(message);
            _taskLoggingHelper.LogMessage(MessageImportance.Low, messageWithNameTag);
        }

        public void LogError(string message)
        {
            string messageWithNameTag = GetMessageWithNameTag(message);
            _taskLoggingHelper.LogError(messageWithNameTag);
        }

        public bool HasLoggedErrors() => _taskLoggingHelper.HasLoggedErrors;

        public string GetMessageWithNameTag(string message)
        {
            string fullMessage = $"[SpecFlow] {message}";
            return fullMessage;
        }
    }
}
