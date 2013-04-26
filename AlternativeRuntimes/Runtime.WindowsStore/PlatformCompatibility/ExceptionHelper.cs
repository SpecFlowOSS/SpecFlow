using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    using System.Reflection;

    internal static class ExceptionHelper
    {
        public static Exception PreserveStackTrace(this Exception ex, string methodInfo = null)
        {
            return ex;
        }
    }
}
