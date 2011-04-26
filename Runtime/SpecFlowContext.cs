using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow
{
    public abstract class SpecFlowContext : Dictionary<string, object>, IDisposable
    {
        protected virtual void Dispose()
        {
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        public bool TryGetValue<TValue>(string key, out TValue value)
        {
            object result;
            if (base.TryGetValue(key, out result))
            {
                value = (TValue)result;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public void Set<T>(T data)
        {
            var id = typeof(T).ToString();
            Set(data, id);
        }

        public void Set<T>(T data, string id)
        {
            this[id] = data;
        }

        public void Set<T>(Func<T> func)
        {
            this[typeof(T).ToString()] = func;
        }

        public T Get<T>()
        {
            var id = typeof(T).ToString();
            return Get<T>(id);
        }

        public T Get<T>(string id)
        {
            var value = this[id];
            if (TheValueIsAFactoryMethod<T>(value))
                value = CallTheFactoryMethodToGetTheValue<T>(value);
            return (T)value;
        }

        private static object CallTheFactoryMethodToGetTheValue<T>(object value)
        {
            value = ((Func<T>) value)();
            return value;
        }

        private static bool TheValueIsAFactoryMethod<T>(object value)
        {
            return value.GetType() == typeof(Func<T>);
        }
    }
}