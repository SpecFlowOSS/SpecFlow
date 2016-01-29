using System.Collections.Generic;

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
    }
}