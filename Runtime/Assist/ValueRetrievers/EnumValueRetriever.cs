using System;
using System.Linq;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class EnumValueRetriever : IValueRetriever<Enum>
    {
        public bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            try
            {
                result = GetValue(context);
                return true;
            }
            catch (InvalidOperationException e)
            {
                if (!e.Message.Contains("No enum with value")) throw;

                result = null;
                return false;
            }
        }

        public object GetValue(ValueRetrieverContext context)
        {
            return GetValue(context.Value, context.InstanceType.GetProperties()
                .First(x => x.Name.MatchesThisColumnName(context.Field))
                .PropertyType);
        }

        public object GetValue(string value, Type enumType)
        {
            CheckThatTheValueIsAnEnum(value, enumType);

            return ConvertTheStringToAnEnum(value, enumType);
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