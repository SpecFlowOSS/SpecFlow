
#if ! BODI_LIMITEDRUNTIME
using System;
using System.Collections.Generic;
using System.Threading;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class ThreadStorage<TClass> : IDisposable
        where TClass : class
    {
        private Dictionary<int, TClass> threadInstanceDictionary = new Dictionary<int, TClass>();
        private ReaderWriterLockSlim dictionaryLock = new ReaderWriterLockSlim();
        private Func<TClass> _constructor;

        public ThreadStorage(Func<TClass> constructor)
        {
            _constructor = constructor;
        }

        public TClass ThreadInstance
        {
            get { return threadValue(); }
            set { threadValue(value); }
        }

        private TClass threadValue(TClass val)
        {
            dictionaryLock.EnterWriteLock();

            try
            {
                if (threadInstanceDictionary.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                {
                    threadInstanceDictionary[Thread.CurrentThread.ManagedThreadId] = val;
                    return null;
                }
                threadInstanceDictionary.Add(Thread.CurrentThread.ManagedThreadId, val);
            }
            finally
            {
                dictionaryLock.ExitWriteLock();
            }
            return val;
        }

        private TClass threadValue()
        {
            dictionaryLock.EnterUpgradeableReadLock();
            try
            {
                TClass instance;

                if (!threadInstanceDictionary.TryGetValue(Thread.CurrentThread.ManagedThreadId, out instance))
                {
                    return threadValue(_constructor());
                }
                return instance;
            }
            finally
            {
                dictionaryLock.ExitUpgradeableReadLock();
            }
        }

        public void Dispose()
        {
            dictionaryLock.EnterWriteLock();
            try
            {
                foreach (var instance in threadInstanceDictionary)
                {
                    if (instance.Value == null)
                        break;

                    var val = instance.Value as IDisposable;

                    if (val == null)
                        break;

                    val.Dispose();
                }

                threadInstanceDictionary.Clear();

                dictionaryLock.Dispose();
            }
            finally
            {
                dictionaryLock.ExitWriteLock();
            }

        }
    }
}
#endif