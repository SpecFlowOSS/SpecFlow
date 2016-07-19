using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace TechTalk.SpecFlow.Reporting
{
    public sealed class AssemblyServices
    {
        static private Assembly LoadAssemblyFrom(string name, string probingFolder) 
        {
            string[] nameParts = name.Split(new char[] {','}, 2);
            string assemblyName = nameParts[0];

            try 
            {
                return Assembly.LoadFrom(Path.Combine(probingFolder, assemblyName) + ".dll");
            }
            catch (Exception) 
            {
                try 
                {
                    return Assembly.LoadFrom(Path.Combine(probingFolder, assemblyName) + ".exe");
                }
                catch (Exception) 
                {
                    return null;
                }
            }
        }
	
        static private Assembly LoadAssemblyFrom(string name, string[] probingPath) 
        {
            foreach (string probingFolder in probingPath)
            {
                Assembly result = LoadAssemblyFrom(name, probingFolder);
                if (result != null)
                    return result;
            }
            return null;
        }

        static public void SubscribeAssemblyResolve(params string[] probingPath) 
        {
            SubscribeAssemblyResolve(AppDomain.CurrentDomain, probingPath);
        }

        static public void SubscribeAssemblyResolve(AppDomain appDomain, params string[] probingPath) 
        {
            AssemblyResolver resolver = new AssemblyResolver(probingPath);
            appDomain.AssemblyResolve += new ResolveEventHandler(resolver.AppDomain_AssemblyResolve);
        }

        [Serializable]
        private class AssemblyResolver
        {
            private string[] probingPath;

            public AssemblyResolver(string[] probingPath) 
            {
                this.probingPath = probingPath;
            }

            public Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs args)
            {
                string nameToResolve = args.Name;

                Assembly result = LoadAssemblyFrom(nameToResolve, probingPath);
                Debug.WriteLine(String.Format("Resolving assembly \"{0}\", result: \"{1}\"",
                                              nameToResolve, 
                                              result == null ? "Failed" : new Uri(result.CodeBase).AbsolutePath));

                return result;
            }
        }
    }
}