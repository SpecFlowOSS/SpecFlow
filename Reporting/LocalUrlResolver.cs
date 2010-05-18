using System;
using System.Xml;

namespace TechTalk.SpecFlow.Reporting
{
    //TODO: check whether this is needed + somehow make it possible that it can fall back to the resource resolver, if one uses the resource:// prefix.
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