using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class GuidValueRetriever
    {
        public virtual Guid GetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new Guid();
            try
            {
                return new Guid(value);
            }
            catch
            {
                try
                {
                    value = value.Replace("-", "");
                    value = value + "00000000000000000000000000000000".Substring(value.Length);

                    return new Guid(value);
                }
                catch
                {
                    return new Guid();
                }
            }
        }
    }
}