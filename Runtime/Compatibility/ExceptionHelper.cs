using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TechTalk.SpecFlow.Compatibility
{
    internal static class ExceptionHelper
    {
        public static Exception PreserveStackTrace(this Exception ex, string methodInfo = null)
        {
            // Mono's implementation of System.Exception doesn't contain the method InternalPreserveStackTrace
            if (MonoHelper.IsMono)
                MonoHelper.PreserveStackTrace(ex);
            else
                typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ex, new object[0]);

            return ex;
        }

    }
}
