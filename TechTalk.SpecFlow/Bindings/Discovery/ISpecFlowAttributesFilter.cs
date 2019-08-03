using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Bindings.Discovery
{
    public interface ISpecFlowAttributesFilter
    {
        IEnumerable<Attribute> FilterForSpecFlowAttributes(IEnumerable<Attribute> customAttributes);
    }
}
