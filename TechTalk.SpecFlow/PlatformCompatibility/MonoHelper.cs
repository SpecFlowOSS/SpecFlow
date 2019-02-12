using System;
using System.Reflection;

namespace TechTalk.SpecFlow.Compatibility
{
	internal class MonoHelper
    {
        internal static bool IsMono { get; private set; }

        static MonoHelper()
        {
            IsMono = Type.GetType("Mono.Runtime") != null;
        }

        internal static void PreserveStackTrace(Exception ex)
        {
            typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ex, ex.StackTrace + Environment.NewLine);
        }

        internal static Assembly GetLoadedAssembly(string assemblyName)
        {
            Assembly locatedAssembly = null;

            // TODO: This may have to change, for now just load the assemblies from the domain
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssemblyName loadedAssemblyName = assembly.GetName();

                if (!loadedAssemblyName.Name.Equals(assemblyName, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                locatedAssembly = assembly;
            }

            return locatedAssembly;
        }
    }
}
