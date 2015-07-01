using System;
using System.Linq;
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

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            var propertyType = targetType.GetProperties().First(x => x.Name.MatchesThisColumnName(row[0])).PropertyType;
            return GetValue(row[1], propertyType);
        }

        public bool CanRetrieve(TableRow row, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return typeof(Enum).IsAssignableFrom(type.GetGenericArguments()[0]);
            return type.IsEnum;
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
                throw new InvalidOperationException(string.Format("No enum with value {0} found", value));
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

        private InvalidOperationException GetInvalidOperationException(string value)
        {
            return new InvalidOperationException(string.Format("No enum with value {0} found", value));
        }
    }
}