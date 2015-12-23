using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Assist
{
    public class TableService : ITableService
    {
        private readonly Config config;
        internal ITableComparisonLogic TableComparisonLogic;
        internal ITableCreationLogic TableCreationLogic;

        public TableService(Config config)
        {
            this.config = config;
            TableCreationLogic = new TableCreationLogic(config);
            TableComparisonLogic = new TableComparisonLogic(this);
        }

        public void CompareToSet<T>(Table table, IEnumerable<T> set)
        {
            TableComparisonLogic.CompareToSet(table, set);
        }

        public void CompareToInstance<T>(Table table, T instance)
        {
            TableComparisonLogic.CompareToInstance(table, instance);
        }

        public IEnumerable<T> CreateSet<T>(Table table)
        {
            return TableCreationLogic.CreateSet<T>(table);
        }

        public IEnumerable<T> CreateSet<T>(Table table, Func<T> methodToCreateEachInstance)
        {
            return TableCreationLogic.CreateSet<T>(table, methodToCreateEachInstance);
        }

        public T CreateInstance<T>(Table table)
        {
            return TableCreationLogic.CreateInstance<T>(table);
        }

        internal IEnumerable<IValueComparer> ValueComparers
        {
            get { return config.ValueComparers; }
        }

        internal IEnumerable<IValueRetriever> ValueRetrievers
        {
            get { return config.ValueRetrievers; }
        }
    }
}