using System;

namespace TechTalk.SpecFlow.CommonModels
{
    public static class Result
    {
        public static IResult Success()
        {
            return new Success();
        }
        public static IResult<T> Success<T>(T result)
        {
            return new Success<T>(result);
        }

        public static IResult Failure(string description)
        {
            return new Failure(description);
        }

        public static IResult Failure(Exception exception)
        {
            return new ExceptionFailure(exception);
        }

        public static IResult Failure(string description, IFailure innerFailure)
        {
            return new WrappedFailure(description, innerFailure);
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

        public static IResult<T> Failure(string description, IFailure innerFailure)
        {
            return new WrappedFailure<T>(description, innerFailure);
        }
    }
}
