using System.Collections;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    internal struct ValueHolder<T> : IEnumerable<T>
    {
        private readonly T _value;

        public bool HasValue { get; }

        public ValueHolder(T value)
        {
            _value = value;
            HasValue = true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (HasValue)
            {
                yield return _value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal static class ValueHolder
    {
        public static ValueHolder<T> Empty<T>()
        {
            return new ValueHolder<T>();
        }

        public static ValueHolder<T> WithValue<T>(T value)
        {
            return new ValueHolder<T>(value);
        }
    }
}