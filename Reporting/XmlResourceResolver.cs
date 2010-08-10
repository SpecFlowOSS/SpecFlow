using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using TechTalk.SpecFlow.Compatibility;

namespace TechTalk.SpecFlow.Reporting
{
    public class XmlResourceResolver : XmlUrlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri == null || !"resource".Equals(absoluteUri.Scheme, StringComparison.InvariantCultureIgnoreCase))
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);

            string resourceName = absoluteUri.AbsolutePath.TrimStart(Path.AltDirectorySeparatorChar).Replace(Path.AltDirectorySeparatorChar, Type.Delimiter);

            Assembly assembly = GetAssemblyFor(absoluteUri.Host);
            Debug.Assert(ofObjectToReturn == null || ofObjectToReturn == typeof(Stream));
			return assembly.GetManifestResourceStream(resourceName);
        }
		
		private static Assembly GetAssemblyFor(string assemblyName)
		{
			// Due to the fact that Uri lowercase's the assembly name and 
			// that *nix file system's are case sensitive we 
			// need to locate the assembly manually and return it
            if (MonoHelper.IsMono)
                return MonoHelper.GetLoadedAssembly(assemblyName);

		    return Assembly.Load(assemblyName);
		}
    }
}
