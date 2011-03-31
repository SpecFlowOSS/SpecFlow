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
    }
}
