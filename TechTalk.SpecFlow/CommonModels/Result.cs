namespace TechTalk.SpecFlow.CommonModels
{
    public abstract class Result<T>
    {
        public static Result<T> Success(T result)
        {
            return new Success<T>(result);
        }

        public static Result<T> Failure()
        {
            return new Failure<T>();
        }
    }
}
