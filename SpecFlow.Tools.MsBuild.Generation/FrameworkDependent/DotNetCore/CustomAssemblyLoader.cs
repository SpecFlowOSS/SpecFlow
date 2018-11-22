using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace SpecFlow.Tools.MsBuild.Generation.FrameworkDependent
{
    internal class CustomAssemblyLoader : AssemblyLoadContext
    {
        protected virtual string ManagedDllDirectory => Path.GetDirectoryName(new Uri(this.GetType().GetTypeInfo().Assembly.CodeBase).LocalPath);

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = Path.Combine(this.ManagedDllDirectory, assemblyName.Name) + ".dll";
            if (File.Exists(assemblyPath))
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return Default.LoadFromAssemblyName(assemblyName);
        }
    }
}