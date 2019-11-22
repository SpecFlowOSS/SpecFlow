using System;
using System.IO;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class ExceptionTaskLogger : IExceptionTaskLogger
    {
        private readonly ITaskLoggingWrapper _taskLoggingWrapper;

        public ExceptionTaskLogger(ITaskLoggingWrapper taskLoggingWrapper)
        {
            _taskLoggingWrapper = taskLoggingWrapper;
        }

        public void LogException(Exception exception)
        {
            if (exception.InnerException is FileLoadException fileLoadException)
            {
                _taskLoggingWrapper.LogError($"FileLoadException Filename: {fileLoadException.FileName}");
                _taskLoggingWrapper.LogError($"FileLoadException FusionLog: {fileLoadException.FusionLog}");
                _taskLoggingWrapper.LogError($"FileLoadException Message: {fileLoadException.Message}");
            }

            if (exception.InnerException is Exception innerException)
            {
                _taskLoggingWrapper.LogError(innerException.ToString());
            }

            _taskLoggingWrapper.LogError(exception.ToString());
        }
    }
}
