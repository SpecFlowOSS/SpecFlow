namespace TechTalk.SpecFlow.Assist
{
    public abstract class ValueRetrieverBase<T> : IValueRetriever<T>
    {
        public abstract bool TryGetValue(ValueRetrieverContext context, out object result);

        public object GetValue(ValueRetrieverContext context)
        {
            object result;
            TryGetValue(context, out result);
            return result;
        }

        public T GetValue(string value)
        {
            return (T) GetValue(new ValueRetrieverContext(value));
        }

        protected delegate bool ParseFunction(string value, out T result);

        protected bool TryParse(ParseFunction parseFunction, string value, out object result)
        {
            T resultObject;
            var success = parseFunction(value, out resultObject);
            result = resultObject;
            return success;
        }
    }
}