using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class EnumValueRetriever
    {
        public object GetValue(string value, Type enumType)
        {
            CheckThatTheValueIsAnEnum(value, enumType);

            return ConvertTheStringToAnEnum(value, enumType);
        }

        private object ConvertTheStringToAnEnum(string value, Type enumType)
        {
            return Enum.Parse(enumType, ParseTheValue(value), true);
        }

        private void CheckThatTheValueIsAnEnum(string value, Type enumType)
        {
            if (value == null)
                throw GetInvalidOperationException("{null}");
            if (value == string.Empty)
                throw GetInvalidOperationException("{empty}");

            try
            {
                ConvertTheStringToAnEnum(value, enumType);
            }
            catch
            {
                throw new InvalidOperationException(string.Format("No enum with value {0} found", value));
            }
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