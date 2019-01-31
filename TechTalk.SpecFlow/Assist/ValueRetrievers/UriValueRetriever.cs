using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UriValueRetriever : NonNullableValueRetriever<Uri>
    {
        public override Uri GetValue(string value)
        {
            return new Uri(value, UriKind.RelativeOrAbsolute);
        }
    }
}
