using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.IdeIntegration.Tracing
{
    public interface IIdeTracer
    {
        void Trace(string message, string category);
        bool IsEnabled(string category);
    }

    public static class TracerExtensions
    {
        public static void Trace(this IIdeTracer tracer, string messageFormat, object category, params object[] messageArgs)
        {
            string categoryMessage = category is string ? category.ToString() : category is Type ? ((Type)category).Name : category.GetType().Name;

            tracer.Trace(string.Format(messageFormat, messageArgs), categoryMessage);
        }
    }
}
