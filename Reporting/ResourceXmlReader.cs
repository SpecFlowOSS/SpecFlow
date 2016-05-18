using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace TechTalk.SpecFlow.Reporting
{
    /// <summary>
    /// XML reader for an embedded resource that uses the resource://assembly/resource base URI.
    /// </summary>
    internal class ResourceXmlReader : XmlTextReader
    {
        private readonly string baseUri;

        public ResourceXmlReader(Type type, string resourceName)
            : this(type.Assembly, type.Namespace + Type.Delimiter + resourceName)
        {

        }

        public ResourceXmlReader(Assembly assembly, string resourceName) 
            : this(assembly, resourceName, new NameTable())
        {
        }

        public ResourceXmlReader(Type type, string resourceName, XmlNameTable nameTable)
            : this(type.Assembly, type.Namespace + Type.Delimiter + resourceName, nameTable)
        {
        }

        public ResourceXmlReader(Assembly assembly, string resourceName, XmlNameTable nameTable)
            : base(GetResourceStream(assembly, resourceName), nameTable)
        {
            var resourcePath =
                Path.GetFileNameWithoutExtension(resourceName).Replace(Type.Delimiter, Path.AltDirectorySeparatorChar) +
                Path.GetExtension(resourceName);
            baseUri = String.Format("resource://{0}/{1}", assembly.GetName().Name, resourcePath);
        }

        private static Stream GetResourceStream(Assembly assembly, string resourceName)
        {
            var result = assembly.GetManifestResourceStream(resourceName);
            if (result == null)
                throw new InvalidOperationException(String.Format("Resource '{0}' not found.", resourceName));
            return result;
        }

        public override string BaseURI
        {
            get
            {
                return baseUri;
            }
        }
    }
}