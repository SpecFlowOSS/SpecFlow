using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class ProjectionExtensionMethods
    {
        public static IEnumerable<Projection<T>> ToProjection<T>(this IEnumerable<T> collection, Table table = null)
        {
            return new EnumerableProjection<T>(table, collection);
        }

        public static IEnumerable<Projection<T>> ToProjection<T>(this Table table)
        {
            return new EnumerableProjection<T>(table);
        }

        public static IEnumerable<Projection<T>> ToProjectionOfSet<T>(this Table table, IEnumerable<T> collection)
        {
            return new EnumerableProjection<T>(table);
        }

        public static IEnumerable<Projection<T>> ToProjectionOfInstance<T>(this Table table, T instance)
        {
            return new EnumerableProjection<T>(table);
        }

        public static void CompareToProjectionOfSet<T>(
            this Table table,
            IEnumerable<T> query,
            Func<IEnumerable<Projection<T>>, IEnumerable<Projection<T>>, bool> compare)
        {
            var tableProjection = table.ToProjectionOfSet(query);
            var queryProjection = query.ToProjection();
            if (!compare(tableProjection, queryProjection))
            {
                throw new ComparisonException(@"Set projections do not match");
            }
        }

        public static void CompareToProjectionOfSet<T>(
            this Table table,
            IEnumerable<T> query,
            Func<IEnumerable<Projection<T>>, IEnumerable<Projection<T>>, IEnumerable<Projection<T>>> diff,
            string identProperty)
        {
            var tableProjection = table.ToProjectionOfSet(query);
            var queryProjection = query.ToProjection();
            var setDifference = diff(tableProjection, queryProjection);
            if (setDifference.Any())
            {
                var idents = string.Join(",", setDifference.Select(x => x[identProperty].ToString()).ToArray());
                throw new ComparisonException(string.Format(@"The following row projections do not match: {0}", idents));
            }
        }

        public static void CompareToProjectionOfInstance<T>(this Table table, T instance)
        {
            var tableProjection = table.ToProjectionOfInstance(instance);
            var instanceProjection = (new List<T>() {instance}).ToProjection();
            if (!tableProjection.Equals(instanceProjection))
            {
                throw new ComparisonException(@"Instance projections do not match");
            }
        }
    }
}
