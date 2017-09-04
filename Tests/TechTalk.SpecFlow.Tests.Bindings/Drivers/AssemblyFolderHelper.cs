using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace TechTalk.SpecFlow.Tests.Bindings.Drivers
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
