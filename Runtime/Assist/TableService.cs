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
    }

    public class TableService : ITableService
    {
        private readonly Config config;
        internal ITableComparisonLogic TableComparisonLogic;
        private readonly TableCreationLogic tableCreationLogic;

        public TableService(Config config)
        {
            this.config = config;
            tableCreationLogic = new TableCreationLogic(config);
            TableComparisonLogic = new TableComparisonLogic(this, tableCreationLogic);
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
            return tableCreationLogic.CreateInstance<T>(table);
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