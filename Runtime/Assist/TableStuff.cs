using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public class TableStuff
    {
        private readonly Config config;

        public TableStuff(Config config)
        {
            this.config = config;
        }

        public T CreateInstance<T>(Table table)
        {
            var helpers = new TableService(config);
            var instanceTable = helpers.GetTheProperInstanceTable(table, typeof (T));
            return helpers.ThisTypeHasADefaultConstructor<T>()
                ? helpers.CreateTheInstanceWithTheDefaultConstructor<T>(instanceTable)
                : helpers.CreateTheInstanceWithTheValuesFromTheTable<T>(instanceTable);
        }

        public T CreateInstance<T>(Table table, Func<T> methodToCreateTheInstance)
        {
            var instance = methodToCreateTheInstance();
            table.FillInstance(instance);
            return instance;
        }

        public void FillInstance(Table table, object instance)
        {
            var helpers = new TableService(config);
            var instanceTable = helpers.GetTheProperInstanceTable(table, instance.GetType());
            helpers.LoadInstanceWithKeyValuePairs(instanceTable, instance);
        }

        public IEnumerable<T> CreateSet<T>(Table table)
        {
            var list = new List<T>();

            var pivotTable = new PivotTable(table);
            for (var index = 0; index < table.Rows.Count(); index++)
            {
                var instance = pivotTable.GetInstanceTable(index).CreateInstance<T>();
                list.Add(instance);
            }

            return list;
        }

        public IEnumerable<T> CreateSet<T>(Table table, Func<T> methodToCreateEachInstance)
        {
            var list = new List<T>();

            var pivotTable = new PivotTable(table);
            for (var index = 0; index < table.Rows.Count(); index++)
            {
                var instance = methodToCreateEachInstance();
                pivotTable.GetInstanceTable(index).FillInstance(instance);
                list.Add(instance);
            }

            return list;
        }
    }

    public class ComparisonTableStuff
    {
        private readonly TableService tableService;

        public ComparisonTableStuff(TableService tableService)
        {
            this.tableService = tableService;
        }

        public void CompareToSet<T>(Table table, IEnumerable<T> set)
        {
            var checker = new SetComparer<T>(table, tableService);
            checker.CompareToSet(set);
        }

        public void CompareToInstance<T>(Table table, T instance)
        {
            AssertThatTheInstanceExists(instance);

            var instanceTable = tableService.GetTheProperInstanceTable(table, typeof (T));

            var differences = FindAnyDifferences(instanceTable, instance);

            if (ThereAreAnyDifferences(differences))
                ThrowAnExceptionThatDescribesThoseDifferences(differences);
        }

        private static void AssertThatTheInstanceExists<T>(T instance)
        {
            if (instance == null)
                throw new ComparisonException("The item to compare was null.");
        }

        private static void ThrowAnExceptionThatDescribesThoseDifferences(IEnumerable<Difference> differences)
        {
            throw new ComparisonException(CreateDescriptiveErrorMessage(differences));
        }

        private static string CreateDescriptiveErrorMessage(IEnumerable<Difference> differences)
        {
            return differences.Aggregate(@"The following fields did not match:",
                (sum, next) => sum + (Environment.NewLine + DescribeTheErrorForThisDifference(next)));
        }

        private static string DescribeTheErrorForThisDifference(Difference difference)
        {
            if (difference.DoesNotExist)
                return string.Format("{0}: Property does not exist", difference.Property);

            return string.Format("{0}: Expected <{1}>, Actual <{2}>",
                difference.Property, difference.Expected,
                difference.Actual);
        }

        private IEnumerable<Difference> FindAnyDifferences<T>(Table table, T instance)
        {
            return from row in table.Rows
                where ThePropertyDoesNotExist(instance, row) || TheValuesDoNotMatch(instance, row)
                select CreateDifferenceForThisRow(instance, row);
        }

        private static bool ThereAreAnyDifferences(IEnumerable<Difference> differences)
        {
            return differences.Count() > 0;
        }

        private bool ThePropertyDoesNotExist<T>(T instance, TableRow row)
        {
            return instance.GetType().GetProperties()
                .Any(property => tableService.IsMemberMatchingToColumnName(property, row.Id())) == false;
        }

        private bool TheValuesDoNotMatch<T>(T instance, TableRow row)
        {
            var expected = GetTheExpectedValue(row);
            var propertyValue = instance.GetPropertyValue(row.Id());

            return tableService.ValueComparers
                .FirstOrDefault(x => x.CanCompare(propertyValue))
                .Compare(expected, propertyValue) == false;
        }

        private static string GetTheExpectedValue(TableRow row)
        {
            return row.Value().ToString();
        }

        private Difference CreateDifferenceForThisRow<T>(T instance, TableRow row)
        {
            if (ThePropertyDoesNotExist(instance, row))
                return new Difference
                {
                    Property = row.Id(),
                    DoesNotExist = true
                };

            return new Difference
            {
                Property = row.Id(),
                Expected = row.Value(),
                Actual = instance.GetPropertyValue(row.Id())
            };
        }

        private class Difference
        {
            public string Property { get; set; }
            public object Expected { get; set; }
            public object Actual { get; set; }
            public bool DoesNotExist { get; set; }
        }
    }
}