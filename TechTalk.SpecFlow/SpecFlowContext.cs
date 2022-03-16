using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow
{
    public interface ISpecFlowContext
    {
        Exception TestError { get; }
    }

    public abstract class SpecFlowContext : Dictionary<string, object>, IDisposable
    {
        public Exception TestError { get; internal set; }

        protected virtual void Dispose()
        {
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        public bool TryGetValue<TValue>(out TValue value)
        {
            return TryGetValue(GetDefaultKey<TValue>(), out value);
        }

        public bool TryGetValue<TValue>(string key, out TValue value)
        {
            if (base.TryGetValue(key, out object result))
            {
                value = TheValueIsAFactoryMethod<TValue>(result) ? CallTheFactoryMethodToGetTheValue<TValue>(result) : (TValue)result;
                return true;
            }

            value = default;
            return false;
        }

        private string GetDefaultKey<T>()
        {
            return typeof(T).FullName;
        }

        public void Set<T>(T data)
        {
            Set(data, GetDefaultKey<T>());
        }

        public void Set<T>(T data, string key)
        {
            this[key] = data;
        }

        public void Set<T>(Func<T> func)
        {
            this[GetDefaultKey<T>()] = func;
        }

        public T Get<T>()
        {
            return Get<T>(GetDefaultKey<T>());
        }

        public T Get<T>(string key)
        {
            var value = this[key];
            if (TheValueIsAFactoryMethod<T>(value))
                value = CallTheFactoryMethodToGetTheValue<T>(value);
            return (T)value;
        }

        private static T CallTheFactoryMethodToGetTheValue<T>(object value)
        {
            return ((Func<T>) value)();
        }

        private static bool TheValueIsAFactoryMethod<T>(object value)
        {
            if (value == null) return false;
            return value.GetType() == typeof(Func<T>);
        }
    }
}