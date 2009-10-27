using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}