using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public class EnumerableProjection<T> : IEnumerable<Projection<T>>
    {
        private readonly IEnumerable<T> collection;
        private readonly IEnumerable<string> properties;

        public EnumerableProjection(Table table, IEnumerable<T> collection = null)
        {
            if (table == null && collection == null)
                throw new ArgumentNullException(nameof(table), "Either table or projectCollection must be specified");

            if (table != null)
                properties = table.Header;
            this.collection = collection ?? table.CreateSet<T>();
        }

        public IEnumerator<Projection<T>> GetEnumerator()
        {
            return new ProjectionEnumerator<T>(collection, properties);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == typeof(T))
            {
                if (collection != null && collection.Count() == 1)
                {
                    return new Projection<T>(collection.First(), properties).Equals(obj);
                }
                return false;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }

    public class ProjectionEnumerator<T> : IEnumerator<Projection<T>>
    {
        private readonly IEnumerator<T> collectionEnumerator;
        private readonly IEnumerable<string> properties;

        public ProjectionEnumerator(IEnumerable<T> collection, IEnumerable<string> properties)
        {
            collectionEnumerator = collection.GetEnumerator();
            this.properties = properties;
        }

        public Projection<T> Current => new Projection<T>(collectionEnumerator.Current, properties);

        public void Dispose()
        {
            collectionEnumerator.Dispose();
        }

        object System.Collections.IEnumerator.Current => Current;

        public bool MoveNext()
        {
            return collectionEnumerator.MoveNext();
        }

        public void Reset()
        {
            collectionEnumerator.Reset();
        }
    }

    public class Projection<T>
    {
        private readonly T item;
        private readonly IEnumerable<string> properties;

        public Projection(T item, IEnumerable<string> properties)
        {
            this.item = item;
            this.properties = properties;
        }

        public override bool Equals(object obj)
        {
            if (obj is Projection<T> otherProjection)
            {
                if (item != null && otherProjection.item != null)
                {
                    IEnumerable<string> properties = this.properties;
                    if (otherProjection.properties != null)
                    {
                        properties = properties?.Intersect(otherProjection.properties) ?? otherProjection.properties;
                    }
                    if (properties != null)
                    {
                        return Compare(item, otherProjection.item, properties);
                    }
                }
            }
            else if (obj != null && obj.GetType() == typeof(T))
            {
                return Compare(item, (T)obj, properties);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return 1;
        }

        private static bool Compare(T t1, T t2, IEnumerable<string> properties)
        {
            foreach (var propertyName in properties)
            {
                var property1 = t1.GetThePropertyOnThisObject(propertyName);
                var property2 = t2.GetThePropertyOnThisObject(propertyName);
                if (property1 == null || property2 == null)
                    return false;

                var value1 = property1.GetValue(t1, null);
                var value2 = property2.GetValue(t2, null);

                if (!Equals(value1, value2))
                    return false;
            }
            return true;
        }
    }
}
