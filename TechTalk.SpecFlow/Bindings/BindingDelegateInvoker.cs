using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Bindings
{
    public class BindingDelegateInvoker : IBindingDelegateInvoker
    {
        public virtual async Task<object> InvokeDelegateAsync(Delegate bindingDelegate, object[] invokeArgs, ExecutionContextHolder executionContext)
        {
            // To be able to simulate the behavior of sequential async or sync steps in a test, we need to ensure that
            // the next step continues with the ExecutionContext that the previous step finished with. 
            //
            // Without preserving the ExecutionContext this would not happen because the async methods all the way up to the 
            // generated test method (for example this method) are discarding the ExecutionContext changes at the end, so the 
            // next step would start with an "empty" ExecutionContext again.
            //
            // It is important that no methods from here (until the user's binding method) is marked with 'async' otherwise that
            // would again discard the ExecutionContext.
            //
            // The ExecutionContext only flows down, so async binding methods cannot directly change it, but even if all binding method
            // is async the constructor of the binding classes are run in sync, so they should be able to change the ExecutionContext. 
            // (The binding classes are created as part of the 'bindingDelegate' this method receives.

            try
            {
                return await InvokeInExecutionContext(executionContext?.Value, () => CreateDelegateInvocationTask(bindingDelegate, invokeArgs));
            }
            finally
            {
                if (executionContext != null)
                    executionContext.Value = ExecutionContext.Capture();
            }
        }

        private Task<object> InvokeInExecutionContext(ExecutionContext executionContext, Func<Task<object>> callback)
        {
            if (executionContext == null)
                return callback();

            Task<object> result = Task.FromResult((object)null);
            ExecutionContext.Run(executionContext, _ => { result = callback(); }, null);
            return result;
        }

        // Important: this method MUST NOT be async because that would discard the ExecutionContext changes during execution!
        private Task<object> CreateDelegateInvocationTask(Delegate bindingDelegate, object[] invokeArgs)
        {
            if (AsyncMethodHelper.IsAwaitable(bindingDelegate.Method.ReturnType))
                return InvokeBindingDelegateAsync(bindingDelegate, invokeArgs);
            return Task.FromResult(InvokeBindingDelegateSync(bindingDelegate, invokeArgs));
        }

        protected virtual object InvokeBindingDelegateSync(Delegate bindingDelegate, object[] invokeArgs)
        {
            return bindingDelegate.DynamicInvoke(invokeArgs);
        }

        // Important: this method MUST NOT be async because that would discard the ExecutionContext changes during execution!
        private Task<object> InvokeBindingDelegateAsync(Delegate bindingDelegate, object[] invokeArgs)
        {
            var result = bindingDelegate.DynamicInvoke(invokeArgs);
            if (AsyncMethodHelper.IsAwaitableAsTask(result, out var task))
                return AsyncMethodHelper.ConvertToTaskOfObject(task);
            return Task.FromResult(result);
        }
    }
}
