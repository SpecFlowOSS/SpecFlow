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

        public static ValueHolder<T> Empty()
        {
            return new ValueHolder<T>();
        }

        public static ValueHolder<T> WithValue(T value)
        {
            return new ValueHolder<T>(value);
        }
    }
}