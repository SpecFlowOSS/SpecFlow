using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;

namespace TechTalk.SpecFlow.Reporting
{
    public class XmlResourceResolver : XmlResolver
    {
        private ICredentials credentials;

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			if (baseUri == null)
			{
				if (relativeUri == null)
					throw new ArgumentNullException ("Either baseUri or relativeUri are required.");
				
				if (relativeUri.StartsWith ("resource:"))
				{
					return new Uri(relativeUri);
				}
				else
				{
					return base.ResolveUri(baseUri, relativeUri);
				}
			}

			return base.ResolveUri(baseUri, relativeUri);
		}
		
		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
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

        public override ICredentials Credentials
        {
            set { credentials = value; }
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