using System;
using System.Collections.Generic;
using Gherkin.Ast;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSources
{
    public interface ISpecificationProvider
    {
        ExternalDataSpecification GetSpecification(IEnumerable<Tag> tags, string sourceFilePath);
    }
}
