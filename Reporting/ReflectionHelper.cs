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
            Type exceptionType = typeof(Exception);
			
			// Mono's implementation of System.Exception doesn't contain the method InternalPreserveStackTrace
			if (Type.GetType("Mono.Runtime") != null)
			{
				exceptionType.GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ex, ex.StackTrace + Environment.NewLine);
			}
			else
			{
				exceptionType.GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ex, new object[0]);
			}
        }
    }
}