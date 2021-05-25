using System;
using System.Collections.Generic;
using Gherkin.Ast;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSource
{
    public interface ISpecificationProvider
    {
        ExternalDataSpecification GetSpecification(IEnumerable<Tag> tags);
    }
}
