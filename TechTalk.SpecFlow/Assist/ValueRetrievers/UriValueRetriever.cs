using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UriValueRetriever : ClassRetriever<Uri>
    {
        protected override Uri GetNonEmptyValue(string value)
        {
            return new Uri(value, UriKind.RelativeOrAbsolute);
        }
    }
}
