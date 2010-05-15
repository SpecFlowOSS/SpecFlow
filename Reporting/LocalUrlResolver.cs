using System;
using System.Xml;

namespace TechTalk.SpecFlow.Reporting
{
    class LocalUrlResolver : XmlUrlResolver
    {
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if (baseUri != null)
                return base.ResolveUri(baseUri, relativeUri);            
            return base.ResolveUri(new Uri("http://mypath/"), relativeUri);
        }
    }
}