using System;
using System.Reflection;

namespace TechTalk.SpecFlow.Reporting
{
    static class ReflectionHelper
    {
        public static T GetProperty<T>(this object source, string propertyName)
        {
            return (T)source.GetType().GetProperty(propertyName).GetValue(source, null);
        }

        public static void PreserveStackTrace(this Exception ex)
        {
            typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ex, new object[0]);
        }
    }
}