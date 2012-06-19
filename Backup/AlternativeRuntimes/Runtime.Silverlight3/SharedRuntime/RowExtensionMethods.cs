using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.Assist
{
    public static class RowExtensionMethods
    {
        public static string GetString(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id)
                       ? row[id]
                       : null;
        }

        public static int GetInt32(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                       ? Convert.ToInt32(row[id])
                       : int.MinValue;
        }

        public static long GetInt64(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                       ? Convert.ToInt64(row[id])
                       : long.MinValue;
        }

        public static decimal GetDecimal(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                       ? Convert.ToDecimal(row[id])
                       : decimal.MinValue;
        }

        public static DateTime GetDateTime(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                       ? Convert.ToDateTime(row[id])
                       : DateTime.MinValue;
        }

        public static bool GetBoolean(this TableRow row, string id)
        {
            if (TheBooleanValueIsEmpty(row, id))
                return false;

            AssertThatTheRequestIsValid(row, id);

            return row[id] == "true";
        }

        public static double GetDouble(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                       ? Convert.ToDouble(row[id])
                       : double.MinValue;
        }

        public static float GetSingle(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                ? Convert.ToSingle(row[id])
                : float.MinValue;
        }

        public static char GetChar(this TableRow row, string id)
        {
            return Convert.ToChar(row[id]);
        }

        internal static Enum GetEnumFromSingleInstanceRow<T>(this TableRow row)
        {
            return GetTheEnumValue<T>(row[1], row[0]);
        }

        public static Enum GetEnum<T>(this TableRow row, string id)
        {
            return GetTheEnumValue<T>(row[id], id);
        }

        private static Enum GetTheEnumValue<T>(string rowValue, string propertyName)
        {
            var value = rowValue.Replace(" ", string.Empty);

            var enumType = GetTheEnumType<T>(propertyName, value);

            return Enum.Parse(enumType, value, true) as Enum;
        }

        private static Type GetTheEnumType<T>(string propertyName, string value)
        {
            var propertyType = (from property in typeof (T).GetProperties()
                                where property.Name == propertyName
                                      && property.PropertyType.IsEnum
                                      && EnumValueIsDefinedCaseInsensitve(property.PropertyType, value)
                                select property.PropertyType).FirstOrDefault();

            if (propertyType == null)
                throw new InvalidOperationException(string.Format("No enum with value {0} found in type {1}", value, typeof(T).Name));

            return propertyType;
        }

        private static bool EnumValueIsDefinedCaseInsensitve(Type @enum, string value)
        {
            Enum parsedEnum = null;
            try
            {
                parsedEnum = Enum.Parse(@enum, value, true) as Enum;
            }
            catch
            {
                // just catch it
            }

            return parsedEnum != null;
        }


        public static Guid GetGuid(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                       ? new Guid(row[id])
                       : new Guid();
        }

        private static bool TheBooleanValueIsEmpty(TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && string.IsNullOrEmpty(row[id]);
        }

        private static bool TheValueIsNotEmpty(TableRow row, string id)
        {
            return string.IsNullOrEmpty(row[id]) == false;
        }

        private static void AssertThatTheRequestIsValid(TableRow row, string id)
        {
            AssertThatAValueWithThisIdExistsInThisRow(row, id);
            AssertThatThisIsAnAcceptableBoolValue(row, id);
        }

        private static void AssertThatThisIsAnAcceptableBoolValue(TableRow row, string id)
        {
            var acceptedValues = new[] { "true", "false" };
            if (acceptedValues.Contains(row[id]) == false)
                throw new InvalidCastException(string.Format("You must use 'true' or 'false' when setting bools for {0}", id));
        }

        private static void AssertThatAValueWithThisIdExistsInThisRow(TableRow row, string id)
        {
            if (AValueWithThisIdExists(row, id) == false)
                throw new InvalidOperationException(string.Format("{0} could not be found in the row.", id));
        }

        private static bool AValueWithThisIdExists(IEnumerable<KeyValuePair<string, string>> row, string id)
        {
            return row.Any(x => x.Key == id);
        }
    }
}
