using System;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public interface IExceptionTaskLogger
    {
        void LogException(Exception exception);
    }
}
