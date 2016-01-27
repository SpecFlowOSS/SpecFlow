using System;
using System.Collections.Generic;

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

        internal IEnumerable<IValueComparer> ValueComparers
        {
            get { return config.ValueComparers; }
        }

        internal IEnumerable<IValueRetriever> ValueRetrievers
        {
            get { return config.ValueRetrievers; }
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
            return TableCreationLogic.CreateSet(table, methodToCreateEachInstance);
        }

        public T CreateInstance<T>(Table table)
        {
            return TableCreationLogic.CreateInstance<T>(table);
        }

        public void FillInstance<T>(Table table, T instance)
        {
            TableCreationLogic.FillInstance(table, instance);
        }
    }
}