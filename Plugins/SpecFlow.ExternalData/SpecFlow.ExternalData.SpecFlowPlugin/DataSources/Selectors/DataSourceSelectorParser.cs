using System;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSources.Selectors
{
    public class DataSourceSelectorParser
    {
        public DataSourceSelector Parse(string selectorExpression)
        {
            return new FieldNameSelector(selectorExpression);
        }
    }
}
