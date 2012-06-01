namespace TechTalk.SpecFlow.Assist
{
    public interface IValueRetriever
    {
        bool TryGetValue(ValueRetrieverContext context, out object result);
        object GetValue(ValueRetrieverContext context);
    }

    public interface IValueRetriever<T> : IValueRetriever
    {
    }
}