namespace TechTalk.SpecFlow.CommonModels
{
    public interface IFailure : IResult
    {
    }

    public interface IFailure<out T> : IResult, IResult<T>
    {
    }
}
