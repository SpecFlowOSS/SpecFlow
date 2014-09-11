using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Bindings.Reflection;
using System.Globalization;
using System.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    internal abstract class TableConverterBase : IStepArgumentTypeConverter
    {
        protected abstract object Convert(Table table, IBindingType typeToConvertTo, CultureInfo cultureInfo);

        protected abstract bool CanConvert(Table table, IBindingType typeToConvertTo, CultureInfo cultureInfo);

        public object Convert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            return Convert((Table)value, typeToConvertTo, cultureInfo);
        }

        public bool CanConvert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            var table = value as Table;
            if (table == null)
                return false;

            return CanConvert(table, typeToConvertTo, cultureInfo);
        }

        protected static bool IsVerticalTable(Table table, RuntimeBindingType runtimeBindingType)
        {
            if (table.Header.Count != 2)
                return false;

            if (table.RowCount == 0)
                return true;

            var writableProperties = GetWritableProperties(runtimeBindingType.Type);
            return writableProperties.ContainsKey(SanitizePropertyName(table.Rows[0][0]));
        }

        protected static Dictionary<string, PropertyInfo> GetWritableProperties(Type type)
        {
            return type.GetProperties().Where(x => x.CanWrite).ToDictionary(x => SanitizePropertyName(x.Name));
        }

        protected static string SanitizePropertyName(string name)
        {
            return name.Replace(" ", "").ToLowerInvariant();
        }
    }
}
