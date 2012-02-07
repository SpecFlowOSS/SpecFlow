using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class GuidValueRetriever
    {
        public virtual Guid GetValue(string value)
        {
            try
            {
                return new Guid(value);
            }
            catch
            {
                try
                {
                    return AttemptToBuildAGuidByAddingTrailingZeroes(value);
                }
                catch
                {
                    return new Guid();
                }
            }
        }

        private static Guid AttemptToBuildAGuidByAddingTrailingZeroes(string value)
        {
            value = value.Replace("-", "");
            value = value + "00000000000000000000000000000000".Substring(value.Length);

            return new Guid(value);
        }

        public bool IsAValidGuid(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            try
            {
                new Guid(value);
                return true;
            }
            catch
            {
                try
                {
                    AttemptToBuildAGuidByAddingTrailingZeroes(value);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}