namespace TechTalk.SpecFlow.CommonModels
{
    public class Success : Result
    {
    }

    public class Success<T> : Success, ISuccess<T>
    {
        public Success(T result)
        {
            Result = result;
        }

        public T Result { get; }
    }
}
