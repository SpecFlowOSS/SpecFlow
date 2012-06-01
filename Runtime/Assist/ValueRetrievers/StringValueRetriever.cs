namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class StringValueRetriever : IValueRetriever<string>
    {
        public string GetValue(string value)
        {
            return value;
        }

        public bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            result = context.Value;
            return true;
        }

        public object GetValue(ValueRetrieverContext context)
        {
            return GetValue(context.Value);
        }
    }
}