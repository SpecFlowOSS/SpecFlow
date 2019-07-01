using System;

namespace TechTalk.SpecFlow.CommonModels
{
    public class ExceptionFailure : IFailure
    {
        public ExceptionFailure(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public Exception Exception { get; }

        public override string ToString() => Exception.ToString();
    }

    public class ExceptionFailure<T> : ExceptionFailure, IFailure<T>
    {
        public ExceptionFailure(Exception exception) : base(exception)
        {
        }
    }
}
