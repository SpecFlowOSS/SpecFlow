namespace TechTalk.SpecFlow.Assist
{
    public class NullableValueRetrieverBase<T> : IValueRetriever<T>
    {
        private readonly IValueRetriever _originalRetriever;

        protected NullableValueRetrieverBase()
        {
            var type = typeof (T).GetGenericArguments()[0];
            _originalRetriever = ScenarioContext.Current.ValueRetrievers.Get(type);
        }

        public bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            if (string.IsNullOrEmpty(context.Value))
            {
                result = null;
                return true;
            }
            return _originalRetriever.TryGetValue(context, out result);
        }

        public object GetValue(ValueRetrieverContext context)
        {
            if (string.IsNullOrEmpty(context.Value)) return null;

            return _originalRetriever.GetValue(context);
        }

        public T GetValue(string value)
        {
            return (T) GetValue(new ValueRetrieverContext(value));
        }
    }
}