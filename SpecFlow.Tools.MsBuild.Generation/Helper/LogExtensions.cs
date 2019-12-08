using System;
using Microsoft.Build.Utilities;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public static class LogExtensions
    {
        public static void LogWithNameTag(
            this TaskLoggingHelper loggingHelper,
            Action<string, object[]> loggingMethod,
            string message,
            params object[] messageArgs)
        {
            string fullMessage = $"[SpecFlow] {message}";
            loggingMethod?.Invoke(fullMessage, messageArgs);
        }
    }
}
