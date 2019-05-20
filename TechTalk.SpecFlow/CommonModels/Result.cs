using System;

namespace TechTalk.SpecFlow.CommonModels
{
    public abstract class Result
    {
        public static Result Success()
        {
            return new Success();
        }

        public static Result Success<T>(T result)
        {
            return new Success<T>(result);
        }

        public static Result Failure()
        {
            return new Failure();
        }

        public static Result Failure(Exception exception)
        {
            return new ExceptionFailure(exception);
        }
    }
}
