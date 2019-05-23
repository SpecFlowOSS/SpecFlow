using System;

namespace TechTalk.SpecFlow.CommonModels
{
    public static class Result
    {
        public static IResult Success()
        {
            return new Success();
        }

        public static IResult Failure(string description)
        {
            return new Failure(description);
        }

        public static IResult Failure(Exception exception)
        {
            return new ExceptionFailure(exception);
        }

        public static IResult Failure(string description, IResult innerResult)
        {
            return new WrappedFailure(description, innerResult);
        }
    }

    public static class Result<T>
    {
        public static IResult<T> Success(T value)
        {
            return new Success<T>(value);
        }

        public static IResult<T> Failure(string description)
        {
            return new Failure<T>(description);
        }

        public static IResult<T> Failure(Exception exception)
        {
            return new ExceptionFailure<T>(exception);
        }

        public static IResult<T> Failure(string description, IResult innerResult)
        {
            return new WrappedFailure<T>(description, innerResult);
        }
    }
}
