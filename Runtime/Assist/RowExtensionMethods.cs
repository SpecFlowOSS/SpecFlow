using System;
using System.Collections.Generic;
using System.Linq;

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
