namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public interface IValueRetriever<T> : IValueRetriever
    {
        T GetValue(string text);
        bool TryGetValue(string text, out T result);
    }
}