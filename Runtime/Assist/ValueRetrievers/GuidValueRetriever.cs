using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class GuidValueRetriever
    {
        public virtual Guid GetValue(string value)
        {
            try
            {
                return new Guid(value);
            }
            catch
            {
                return new Guid();
            }
        }
    }
}