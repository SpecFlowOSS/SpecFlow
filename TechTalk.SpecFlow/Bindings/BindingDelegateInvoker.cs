using System;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Bindings
{
    public class BindingDelegateInvoker : IBindingDelegateInvoker
    {
        public virtual async Task<object> InvokeDelegateAsync(Delegate bindingDelegate, object[] invokeArgs)
        {
            if (typeof(Task).IsAssignableFrom(bindingDelegate.Method.ReturnType))
                return await InvokeBindingDelegateAsync(bindingDelegate, invokeArgs);
            return InvokeBindingDelegateSync(bindingDelegate, invokeArgs);
        }

        protected virtual object InvokeBindingDelegateSync(Delegate bindingDelegate, object[] invokeArgs)
        {
            return bindingDelegate.DynamicInvoke(invokeArgs);
        }

        protected virtual async Task<object> InvokeBindingDelegateAsync(Delegate bindingDelegate, object[] invokeArgs)
        {
            var r = bindingDelegate.DynamicInvoke(invokeArgs);
            if (r is Task t)
                await t;
            return r;
        }
    }
}
