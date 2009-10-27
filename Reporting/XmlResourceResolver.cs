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

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            string resourceName = absoluteUri.AbsolutePath.TrimStart(Path.AltDirectorySeparatorChar).Replace(Path.AltDirectorySeparatorChar, Type.Delimiter);
            string assemblyName = absoluteUri.Host;

            Assembly assembly = Assembly.Load(assemblyName);
            return new ResourceXmlReader(assembly, resourceName);
        }

        public override ICredentials Credentials
        {
            set { credentials = value; }
        }
    }
}