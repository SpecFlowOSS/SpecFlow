using System;

namespace TechTalk.SpecFlow.CommonModels
{
    public static class Result
    {
        public static IResult Success()
        {
            return new Success();
        }

        public static IResult Failure()
        {
            return new Failure();
        }

        public static IResult Failure(Exception exception)
        {
            return new ExceptionFailure(exception);
        }
    }

    public static class Result<T>
    {
        public static IResult<T> Success(T value)
        {
            return new Success<T>(value);
        }

        public static IResult<T> Failure()
        {
            return new Failure<T>();
        }

        public static IResult<T> Failure(Exception exception)
        {
            return new ExceptionFailure<T>(exception);
        }
    }
}
