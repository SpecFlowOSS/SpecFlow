namespace TechTalk.SpecFlow.CommonModels
{
    public class Success : ISuccess
    {
    }

    public class Success<T> : ISuccess<T>
    {
        public Success(T result)
        {
            Result = result;
        }

        public T Result { get; }
    }
}
