using System;

namespace TechTalk.SpecFlow.CommonModels
{
    public class ExceptionFailure<T> : Failure<T>
    {
        public ExceptionFailure(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public Exception Exception { get; }
    }
}
