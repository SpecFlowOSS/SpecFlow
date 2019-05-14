namespace TechTalk.SpecFlow.CommonModels
{
    public class Success<T> : Result<T>
    {
        public Success(T result)
        {
            Result = result;
        }

        public T Result { get; }
    }
}
