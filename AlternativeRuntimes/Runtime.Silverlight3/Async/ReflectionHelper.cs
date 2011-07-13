using System;
using System.Reflection;

namespace TechTalk.SpecFlow.Async
{
    // Yikes!
    internal class ReflectionHelper
    {
        private readonly PropertyInfo unitTestHarnessProperty;
        private readonly PropertyInfo dispatcherStackProperty;

        private readonly MethodInfo enqueueWorkItemMethod;
        private readonly MethodInfo pushWorkItemMethod;
        private readonly MethodInfo popWorkItemMethod;

        private readonly Type compositeWorkItemType;

        private readonly EventInfo compositeWorkItemExceptionEvent;

        public ReflectionHelper(Type workItemTestType)
        {
            enqueueWorkItemMethod = workItemTestType.GetMethod("EnqueueWorkItem");

            unitTestHarnessProperty = workItemTestType.GetProperty("UnitTestHarness");

            var unitTestHarnessType = unitTestHarnessProperty.PropertyType;

            dispatcherStackProperty = unitTestHarnessType.GetProperty("DispatcherStack");

            var dispatcherStackType = dispatcherStackProperty.PropertyType;

            pushWorkItemMethod = dispatcherStackType.GetMethod("Push");
            popWorkItemMethod = dispatcherStackType.GetMethod("Pop");

            compositeWorkItemType = pushWorkItemMethod.GetParameters()[0].ParameterType;
            compositeWorkItemExceptionEvent = compositeWorkItemType.GetEvent("UnhandledException");
        }

        public void EnqueueWorkItem(object silverlightTestInstance, object workItem)
        {
            enqueueWorkItemMethod.Invoke(silverlightTestInstance, new[] { workItem });
        }

        public void PushDefaultWorkItem(object silverlightTestInstance, object workItem)
        {
            var dispatcherStackInstance = GetDispatcherStackInstance(silverlightTestInstance);
            pushWorkItemMethod.Invoke(dispatcherStackInstance, new[] { workItem });
        }

        public void PopDefaultWorkItem(object silverlightTestInstance)
        {
            var dispatcherStackInstance = GetDispatcherStackInstance(silverlightTestInstance);
            popWorkItemMethod.Invoke(dispatcherStackInstance, null);
        }

        private object GetDispatcherStackInstance(object silverlightTestInstance)
        {
            var unitTestHarnessInstance = unitTestHarnessProperty.GetValue(silverlightTestInstance, null);
            return dispatcherStackProperty.GetValue(unitTestHarnessInstance, null);
        }

        public object CreateCompositeWorkItem(Action<Exception> onUnhandledException)
        {
            var handler = new OnUnhandledExceptionHandler(onUnhandledException);
            var @delegate = Delegate.CreateDelegate(compositeWorkItemExceptionEvent.EventHandlerType, handler,
                                                    typeof(OnUnhandledExceptionHandler).GetMethod("OnUnhandledException"));

            var compositeWorkItem = Activator.CreateInstance(compositeWorkItemType);
            compositeWorkItemExceptionEvent.AddEventHandler(compositeWorkItem, @delegate);
            return compositeWorkItem;
        }

        private class OnUnhandledExceptionHandler
        {
            private readonly Action<Exception> onUnhandledException;

            public OnUnhandledExceptionHandler(Action<Exception> onUnhandledException)
            {
                this.onUnhandledException = onUnhandledException;
            }

            public void OnUnhandledException(object sender, object args)
            {
                // args is of type UnhandledExceptionEventArgs
                var exception = args.GetType().GetProperty("ExceptionObject").GetValue(args, null) as Exception;

                onUnhandledException(exception);

                // Drain the remaining work items from the currently nested composite before rethrowing
                // (we've had an exception, it's pointless continuing in an unknown state)
                // TODO: What happens to the pushed work item at this point?
                var compositeWorkItem = sender;

                var type = compositeWorkItem.GetType();
                var remainingWork = type.GetProperty("RemainingWork");
                var dequeue = type.GetMethod("Dequeue");

                while ((bool)remainingWork.GetValue(compositeWorkItem, null))
                    dequeue.Invoke(compositeWorkItem, null);
            }
        }
    }
}