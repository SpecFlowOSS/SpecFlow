using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class GuidValueRetriever : IValueRetriever<Guid>
    {
        public bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            if (IsAValidGuid(context.Value))
            {
                result = GetValue(context.Value);
                return true;
            }
            result = default(Guid);
            return false;
        }

        public object GetValue(ValueRetrieverContext context)
        {
            return GetValue(context.Value);
        }

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