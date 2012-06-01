using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public class ValueRetrieverCollection : Dictionary<Type, Type>
    {
        public void Set<TType, TValueRetriever>() where TValueRetriever : IValueRetriever<TType>
        {
            this[typeof (TType)] = typeof (TValueRetriever);
        }

        public IValueRetriever<T> Get<T>()
        {
            return (IValueRetriever<T>) Get(typeof (T));
        }

        public IValueRetriever Get(Type type)
        {
            if (!this.ContainsKey(type))
                throw new KeyNotFoundException("No value retriever for " + type.Name);

            return (IValueRetriever) Activator.CreateInstance(this[type]);
        }

        public object GetValue<T>(ValueRetrieverContext context)
        {
            var valueRetriever = Get<T>();
            return valueRetriever.GetValue(context);
        }

        public object GetValue(Type type, ValueRetrieverContext context)
        {
            var valueRetriever = Get(type);
            return valueRetriever.GetValue(context);
        }

        public bool TryGetValue<T>(ValueRetrieverContext context, out object result)
        {
            var valueRetriever = Get<T>();
            return valueRetriever.TryGetValue(context, out result);
        }
    }
}