using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class EnumValueRetriever : IValueRetriever
    {
        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType.IsEnum || (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && propertyType.GetGenericArguments()[0].IsEnum);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            var value = keyValuePair.Value;

            if (propertyType.IsEnum)
            {
                if (value == null)
                {
                    throw new InvalidOperationException("No enum with value {null} found");
                }

                if (value.Length == 0)
                {
                    throw new InvalidOperationException("No enum with value {empty} found");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }

                propertyType = propertyType.GetGenericArguments()[0];
            }

            try
            {
                return StepArgumentTypeConverter.ConvertToAnEnum(propertyType, value);
            }
            catch
            {
                throw new InvalidOperationException($"No enum with value {value} found");
            }
        }
    }
}