using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class GuidValueRetriever : StructRetriever<Guid>
    {
        protected override Guid GetNonEmptyValue(string value)
        {
            if (Guid.TryParse(value, out var result))
            {
                return result;
            }
            
            AttemptToBuildAGuidByAddingTrailingZeroes(value, out result);
            return result;
        }

        public Guid GetValue(string value)
        {
            return this.GetNonEmptyValue(value);
        }

        private static bool AttemptToBuildAGuidByAddingTrailingZeroes(string value, out Guid result)
        {
            if (value == null)
            {
                result = Guid.Empty;
                return false;
            }

            const int CharsInGuid = 32;
            value = value.Replace("-", "").PadRight(CharsInGuid, '0');

            return Guid.TryParse(value, out result);
        }

        public bool IsAValidGuid(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (Guid.TryParse(value, out _))
            {
                return true;
            }

            return AttemptToBuildAGuidByAddingTrailingZeroes(value, out _);
        }
    }
}
