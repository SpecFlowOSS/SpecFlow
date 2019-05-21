namespace TechTalk.SpecFlow.CommonModels
{
    public interface ISuccess : IResult
    {
    }

    public interface ISuccess<out T> : ISuccess, IResult<T>
    {
        T Result { get; }
    }
}
