using System;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Bindings
{
    public interface ISynchronousBindingDelegateInvoker
    {
        object InvokeDelegateSynchronously(Delegate bindingDelegate, object[] invokeArgs);
    }

    public class SynchronousBindingDelegateInvoker : ISynchronousBindingDelegateInvoker
    {
        public virtual object InvokeDelegateSynchronously(Delegate bindingDelegate, object[] invokeArgs)
        {
            if (typeof(Task).IsAssignableFrom(bindingDelegate.Method.ReturnType))
                return InvokeBindingDelegateAsync(bindingDelegate, invokeArgs);
            return InvokeBindingDelegateSync(bindingDelegate, invokeArgs);
        }

        protected virtual object InvokeBindingDelegateSync(Delegate bindingDelegate, object[] invokeArgs)
        {
            return bindingDelegate.DynamicInvoke(invokeArgs);
        }

        protected virtual object InvokeBindingDelegateAsync(Delegate bindingDelegate, object[] invokeArgs)
        {
            return AsyncHelpers.RunSync(async () =>
            {
                var r = bindingDelegate.DynamicInvoke(invokeArgs);
                if (r is Task t)
                    await t;
                return r;
            });
        }

    }
}
