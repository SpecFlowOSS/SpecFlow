using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class GuidValueRetriever
    {
        public virtual Guid GetValue(string value)
        {
            var cleanedValue = "";
            try
            {
                cleanedValue = RemoveUnnecessaryCharacters(value);
                return AttempToBuildAGuidFromTheString(value);
            }
            catch
            {
                try
                {
                    return AttemptToBuildAGuidByAddingHeadingZeroes(cleanedValue);
                }
                catch
                {
                    return new Guid();
                }
            }
        }

        private static Guid AttempToBuildAGuidFromTheString(string value)
        {
            var guid = new Guid(value);

            if (RemoveUnnecessaryCharacters(guid) != value)
                throw new Exception("The parsed value is not what was expected.");

            return guid;
        }

        private static string RemoveUnnecessaryCharacters(object value)
        {
            return value.ToString().Replace("{", "").Replace("}", "").Replace("-", "");
        }

        private static Guid AttemptToBuildAGuidByAddingHeadingZeroes(string value)
        {
            value = value.Replace("-", "");
            value = "00000000000000000000000000000000".Substring(value.Length) + value;

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
                    AttemptToBuildAGuidByAddingHeadingZeroes(value);
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
