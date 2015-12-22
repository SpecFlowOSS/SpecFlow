using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Assist
{
    public interface ITableService
    {
        void CompareToSet<T>(Table table, IEnumerable<T> set);
        void CompareToInstance<T>(Table table, T instance);
        T CreateInstance<T>(Table table);
    }

    public class TableService : ITableService
    {
        private readonly Config config;
        internal ITableComparisonLogic TableComparisonLogic;
        internal ITableCreationLogic TableCreationLogic;

        public TableService(Config config)
        {
            this.config = config;
            TableCreationLogic = new TableCreationLogic(config, this);
            TableComparisonLogic = new TableComparisonLogic(this, TableCreationLogic);
        }

        public void CompareToSet<T>(Table table, IEnumerable<T> set)
        {
            TableComparisonLogic.CompareToSet(table, set);
        }

        public void CompareToInstance<T>(Table table, T instance)
        {
            TableComparisonLogic.CompareToInstance(table, instance);
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