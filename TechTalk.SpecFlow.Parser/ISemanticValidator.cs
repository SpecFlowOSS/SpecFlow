using System.Collections.Generic;

namespace TechTalk.SpecFlow.Parser
{
    internal interface ISemanticValidator
    {
        List<SemanticParserException> Validate(SpecFlowFeature feature);
    }
}