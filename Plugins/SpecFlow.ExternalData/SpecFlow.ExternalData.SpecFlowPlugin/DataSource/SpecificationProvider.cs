using System;
using System.Collections.Generic;
using System.Linq;
using Gherkin.Ast;
using SpecFlow.ExternalData.SpecFlowPlugin.Loaders;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSource
{
    public class SpecificationProvider : ISpecificationProvider
    {
        private const string DATASOURCE_TAG_PREFIX = "@DataSource:";

        public ExternalDataSpecification GetSpecification(IEnumerable<Tag> tags)
        {
            var dataSourceTag = tags.FirstOrDefault(t => t.Name.StartsWith(DATASOURCE_TAG_PREFIX));
            if (dataSourceTag == null) 
                return null;

            var dataSourcePath = dataSourceTag.Name.Substring(DATASOURCE_TAG_PREFIX.Length);
            //TODO: decide on loader
            var loader = new CsvLoader();
            //TODO: get feature info
            var dataSource = loader.LoadDataSource(dataSourcePath, null, null);
            //TODO: verify?
            return new ExternalDataSpecification(dataSource);
        }
    }
}
