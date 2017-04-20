using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TechTalk.SpecFlow.Compatibility
{
    public class MonoHelper
    {
        public static bool IsMono { get; private set; }

        static MonoHelper()
        {
            IsMono = Type.GetType("Mono.Runtime") != null;
        }

        public static void PreserveStackTrace(Exception ex)
        {
            typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ex, ex.StackTrace + Environment.NewLine);
        }
    }
}
