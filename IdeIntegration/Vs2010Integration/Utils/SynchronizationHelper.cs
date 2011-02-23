using System;
using System.Collections;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Vs2010Integration.Utils
{
    public class SynchronizedResultCache<TSource, TKey, TValue> where TValue : class
    {
        private readonly Dictionary<TKey, TValue> innerDictionary = new Dictionary<TKey, TValue>();
        private readonly Func<TSource, TValue> initializer;
        private readonly Func<TSource, TKey> getKey;

        public SynchronizedResultCache(Func<TSource, TValue> initializer, Func<TSource, TKey> getKey)
        {
            this.initializer = initializer;
            this.getKey = getKey;
        }

        public bool ContainsResult(TSource source)
        {
            return innerDictionary.ContainsKey(getKey(source));
        }

        public TValue GetOrCreate(TSource source)
        {
            TValue result;
            var key = getKey(source);
            if (!innerDictionary.TryGetValue(key, out result))
            {
                lock (this)
                {
                    if (!innerDictionary.TryGetValue(key, out result))
                    {
                        result = initializer(source);
                        System.Threading.Thread.MemoryBarrier();
                        innerDictionary.Add(key, result);
                    }
                }
            }
            return result;
        }

        public void Clear()
        {
            innerDictionary.Clear();
        }

        public ICollection<TValue> Values
        {
            get { return innerDictionary.Values; }
        }
    }

    public class SynchInitializedInstance<T> where T : class
    {
        private T instance = null;

        private readonly Func<T> initializer;

        public SynchInitializedInstance(Func<T> initializer)
        {
            this.initializer = initializer;
        }

        public bool IsInitialized
        {
            get { return instance != null; }
        }

        public void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                lock (this)
                {
                    if (!IsInitialized)
                    {
                        var newInstance = initializer();
                        System.Threading.Thread.MemoryBarrier();
                        instance = newInstance;
                    }
                }
            }
        }

        public T Value
        {
            get
            {
                EnsureInitialized();
                return instance;
            }
            set
            {
                lock (this)
                {
                    instance = value;
                }
            }
        }
    }
}
