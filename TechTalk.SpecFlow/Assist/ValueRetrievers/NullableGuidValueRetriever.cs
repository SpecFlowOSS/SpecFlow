using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableGuidValueRetriever : IValueRetriever
    {
        private readonly Func<string, Guid> guidValueRetriever = v => new GuidValueRetriever().GetValue(v);

        public NullableGuidValueRetriever(Func<string, Guid> guidValueRetriever = null)
        {
            if (guidValueRetriever != null)
                this.guidValueRetriever = guidValueRetriever;
        }

        public virtual Guid? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return guidValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(Guid?);
        }
    }
}