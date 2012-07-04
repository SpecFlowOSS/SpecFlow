using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Compatibility
{
    internal static class EnumHelper
    {
        public static Array GetValues(Type type)
        {
            return Enum.GetValues(type);
        }

        public static string[] GetNames(Type type)
        {
            return Enum.GetNames(type);
        }
    }

    internal static class TypeHelper
    {
        public static bool IsNested(Type type)
        {
            return type.IsNested;
        }
    }
}
