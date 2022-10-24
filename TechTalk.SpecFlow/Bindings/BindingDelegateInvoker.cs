using System;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Bindings
{
    public class BindingDelegateInvoker : IBindingDelegateInvoker
    {
        public virtual async Task<object> InvokeDelegateAsync(Delegate bindingDelegate, object[] invokeArgs)
        {
            if (AsyncMethodHelper.IsAwaitable(bindingDelegate.Method.ReturnType))
                return await InvokeBindingDelegateAsync(bindingDelegate, invokeArgs);
            return InvokeBindingDelegateSync(bindingDelegate, invokeArgs);
        }

        protected virtual object InvokeBindingDelegateSync(Delegate bindingDelegate, object[] invokeArgs)
        {
            return bindingDelegate.DynamicInvoke(invokeArgs);
        }

        protected virtual async Task<object> InvokeBindingDelegateAsync(Delegate bindingDelegate, object[] invokeArgs)
        {
            var result = bindingDelegate.DynamicInvoke(invokeArgs);
            if (AsyncMethodHelper.IsAwaitableAsTask(result, out var task))
                await task;
            return result;
        }
    }
}
