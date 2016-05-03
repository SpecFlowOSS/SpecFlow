using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class EnumValueRetriever : IValueRetriever
    {
        public object GetValue(string value, Type enumType)
        {
            CheckThatTheValueIsAnEnum(value, enumType);

            return ConvertTheStringToAnEnum(value, enumType);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value, propertyType);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                return typeof(Enum).IsAssignableFrom(propertyType.GetGenericArguments()[0]);
            return propertyType.IsEnum;
        }

        private object ConvertTheStringToAnEnum(string value, Type enumType)
        {
            return Enum.Parse(GetTheEnumType(enumType), ParseTheValue(value), true);
        }

        private static Type GetTheEnumType(Type enumType)
        {
            return ThisIsNotANullableEnum(enumType) ? enumType : enumType.GetGenericArguments()[0];
        }

        private void CheckThatTheValueIsAnEnum(string value, Type enumType)
        {
            if (ThisIsNotANullableEnum(enumType))
                CheckThatThisNotAnObviouslyIncorrectNonNullableValue(value);

            try
            {
                ConvertTheStringToAnEnum(value, enumType);
            }
            catch
            {
                throw new InvalidOperationException($"No enum with value {value} found");
            }
        }

        private void CheckThatThisNotAnObviouslyIncorrectNonNullableValue(string value)
        {
            if (value == null)
                throw GetInvalidOperationException("{null}");
            if (value == string.Empty)
                throw GetInvalidOperationException("{empty}");
        }

        private static bool ThisIsNotANullableEnum(Type enumType)
        {
            return enumType.IsGenericType == false;
        }

        private string ParseTheValue(string value)
        {
            value = value.Replace(" ", "");
            return value;
        }

        private InvalidOperationException GetInvalidOperationException(string value) => new InvalidOperationException($"No enum with value {value} found");
    }
}