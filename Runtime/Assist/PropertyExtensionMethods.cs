using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TechTalk.SpecFlow.Assist
{
    internal static class PropertyExtensionMethods
    {
        public static object GetPropertyValue(this object @object, string propertyName)
        {
            var property = GetThePropertyOnThisObject(@object, propertyName);
            return property.GetValue(@object, null);
        }

        public static void SetPropertyValue(this object @object, string propertyName, object value)
        {
            var property = GetThePropertyOnThisObject(@object, propertyName);
            property.SetValue(@object, value, null);
        }

        private static PropertyInfo GetThePropertyOnThisObject(object @object, string propertyName)
        {
            return @object.GetType().GetProperty(propertyName);
        }
    }
}
