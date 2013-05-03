using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Assist
{
    public class EnumerableProjection<T> : IEnumerable<Projection<T>>
    {
        private IEnumerable<T> collection;
        private IEnumerable<string> properties;

        public EnumerableProjection(Table table, IEnumerable<T> collection = null)
        {
            if (table == null && collection == null)
                throw new ArgumentNullException("table", "Either table or projectCollection must be specified");

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
                else
                {
                    return false;
                }
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
        private IEnumerator<T> collectionEnumerator;
        private IEnumerable<string> properties;

        public ProjectionEnumerator(IEnumerable<T> collection, IEnumerable<string> properties)
        {
            collectionEnumerator = collection.GetEnumerator();
            this.properties = properties;
        }

        public Projection<T> Current
        {
            get
            {
                return new Projection<T>(collectionEnumerator.Current, properties);
            }
        }

        public void Dispose()
        {
            collectionEnumerator.Dispose();
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

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

        public T Value
        {
            get { return item; }
        }

        public object this[string key]
        {
            get { return item.GetPropertyValue(key); }
        }

        public override bool Equals(object obj)
        {
            if (obj is Projection<T>)
            {
                var otherProjection = obj as Projection<T>;
                if (item != null && otherProjection.item != null)
                {
                    var properties = this.properties;
                    if (otherProjection.properties != null)
                    {
                        if (properties == null)
                            properties = otherProjection.properties;
                        else
                            properties = properties.Intersect(otherProjection.properties);
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
            foreach (var property in properties)
            {
                if (t1.GetType().GetProperty(property) == null || t2.GetType().GetProperty(property) == null)
                    return false;

                var thisValue = t1.GetPropertyValue(property);
                var otherValue = t2.GetPropertyValue(property);
                if (thisValue != null)
                {
                    if (otherValue == null)
                        return false;

                    if (thisValue.ToString() != otherValue.ToString())
                        return false;
                }
                else
                {
                    if (otherValue != null)
                        return false;
                }
            }
            return true;
        }
    }
}
