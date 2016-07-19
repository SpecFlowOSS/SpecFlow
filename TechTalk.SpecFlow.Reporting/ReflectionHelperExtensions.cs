using System;
using System.Reflection;

namespace TechTalk.SpecFlow.Reporting
{
    static class ReflectionHelperExtensions
    {
        public static T GetProperty<T>(this object source, string propertyName)
        {
            return (T)source.GetType().GetProperty(propertyName).GetValue(source, null);
        }
    }
}