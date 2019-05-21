namespace TechTalk.SpecFlow.CommonModels
{
    public class Failure : IFailure
    {
    }

    public class Failure<T> : Failure, IFailure<T>
    {
    }
}
