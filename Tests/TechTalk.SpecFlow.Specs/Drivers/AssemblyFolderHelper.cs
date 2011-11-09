using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public static class AssemblyFolderHelper
    {
        public static string GetTestAssemblyFolder()
        {
            var assemblyFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            Debug.Assert(assemblyFolder != null);
            return assemblyFolder;
        }
    }
}
