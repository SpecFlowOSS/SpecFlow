using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class GuidValueRetriever : IValueRetriever
    {
        public virtual Guid GetValue(string value)
        {
            var cleanedValue = "";
            try
            {
                cleanedValue = RemoveUnnecessaryCharacters(value);
                return AttemptToBuildAGuidFromTheString(cleanedValue);
            }
            catch
            {
                try
                {
                    return AttemptToBuildAGuidByAddingTrailingZeroes(cleanedValue);
                }
                catch
                {
                    return new Guid();
                }
            }
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(Guid);
        }

        private static Guid AttemptToBuildAGuidFromTheString(string value)
        {
            var guid = new Guid(value);

            if (string.Compare(RemoveUnnecessaryCharacters(guid), value, true) != 0)
                throw new Exception("The parsed value is not what was expected.");

            return guid;
        }

        private static string RemoveUnnecessaryCharacters(object value)
        {
            return value.ToString().Replace("{", "").Replace("}", "").Replace("-", "");
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
