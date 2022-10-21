using System;
using System.Threading;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Bindings
{
    public class BindingDelegateInvoker : IBindingDelegateInvoker
    {
#if NETCOREAPP
        private ExecutionContext _executionContext = null;
#endif

        public virtual async Task<object> InvokeDelegateAsync(Delegate bindingDelegate, object[] invokeArgs)
        {
#if NETCOREAPP
            if (_executionContext == null)
                _executionContext = ExecutionContext.Capture();
            else
                ExecutionContext.Restore(_executionContext);
#endif        
            try
            {
                if (AsyncMethodHelper.IsAwaitable(bindingDelegate.Method.ReturnType))
                    return await InvokeBindingDelegateAsync2(bindingDelegate, invokeArgs);
                return InvokeBindingDelegateSync(bindingDelegate, invokeArgs);
            }
            finally
            {
#if NETCOREAPP
                _executionContext = ExecutionContext.Capture();
#endif
            }
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

        protected virtual Task<object> InvokeBindingDelegateAsync2(Delegate bindingDelegate, object[] invokeArgs)
        {
            var result = bindingDelegate.DynamicInvoke(invokeArgs);
            if (AsyncMethodHelper.IsAwaitableAsTask(result, out var task))
            {
                if (task is Task<object> taskOfObj)
                    return taskOfObj;
                return task.ContinueWith(_ => (object)null);
            }
            return Task.FromResult(result);
        }
    }
}
