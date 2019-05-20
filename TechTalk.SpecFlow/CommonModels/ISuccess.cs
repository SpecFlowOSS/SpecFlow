namespace TechTalk.SpecFlow.CommonModels
{
    public interface ISuccess<out T>
    {
        T Result { get; }
    }
}
