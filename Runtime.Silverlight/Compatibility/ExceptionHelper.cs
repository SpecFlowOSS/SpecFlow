using System;

namespace TechTalk.SpecFlow.Compatibility
{
    internal static class ExceptionHelper
    {
        public static Exception PreserveStackTrace(this Exception ex, string stepName = null)
        {
            // Silverlight can't do the private reflection required to preserve the
            // stack trace, and since we have to store and rethrow this exception,
            // we have no choice but to wrap it, but make sure we don't wrap it more
            // than once
            if (!(ex is ScenarioErrorException))
                ex = new ScenarioErrorException(stepName, ex);
            return ex;
        }

        private class ScenarioErrorException : Exception
        {
            public ScenarioErrorException(string stepDetails, Exception innerException)
                : base(FormatMessage(stepDetails, innerException), innerException)
            {
            }

            private static string FormatMessage(string stepName, Exception innerException)
            {
                return string.Format("Exception thrown calling step {0}:{1}<{2}>", stepName, Environment.NewLine, innerException.Message);
            }
        }
    }
}
