using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TechTalk.SpecFlow.Compatibility
{
    internal static class EnumHelper
    {
        public static Array GetValues(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            var values = new List<object>();

            var fields = from field in enumType.GetFields()
                         where field.IsLiteral
                         select field;

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(enumType);
                values.Add(value);
            }

            return values.ToArray();
        }

        public static string[] GetNames(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            FieldInfo[] fiArray = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            return fiArray.Select(fi => fi.Name).ToArray();
        }
    }

    internal static class TypeHelper
    {
        public static bool IsNested(Type type)
        {
            return type.DeclaringType != null;
        }
    }
}
