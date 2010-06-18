using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;

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
			
			if (ofObjectToReturn != null)
			{
				if (ofObjectToReturn == typeof(ResourceXmlReader) || ofObjectToReturn == typeof(XmlTextReader))
				{	
					return new ResourceXmlReader(assembly, resourceName);
				}
				else if (ofObjectToReturn == typeof(Stream))
				{
					return assembly.GetManifestResourceStream(resourceName);
				}
			}
			
			return assembly.GetManifestResourceStream(resourceName);
        }
		
		private static Assembly GetAssemblyFor(string host)
		{
			Assembly locatedAssembly = null;
			
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssemblyName assemblyName = assembly.GetName();
				
				if (!assemblyName.Name.Equals(host, StringComparison.CurrentCultureIgnoreCase))
					continue;
				
				locatedAssembly = assembly;
			}
			
			return locatedAssembly;
		}
    }
}
