using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableGuidValueRetriever : IValueRetriever<Guid?>
    {
        private readonly IValueRetriever<Guid> guidValueRetriever;

        public NullableGuidValueRetriever(IValueRetriever<Guid> guidValueRetriever)
        {
            this.guidValueRetriever = guidValueRetriever;
        }

        public Guid? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return guidValueRetriever.GetValue(value);
        }

        public bool TryGetValue(string text, out Guid? result)
        {
            if (string.IsNullOrEmpty(text))
            {
                result = null;
                return true;
            }
            Guid original;
            var tryResult = guidValueRetriever.TryGetValue(text, out original);
            result = original;
            return tryResult;
        }
    }
}