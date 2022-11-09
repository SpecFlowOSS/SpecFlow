using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    internal static class AsyncMethodHelper
    {
        private static bool IsTask(Type type)
        {
            return typeof(Task).IsAssignableFrom(type);
        }

        private static bool IsTaskOfT(Type type, out Type typeArg)
        {
            while (type != null)
            {
                var isTaskOfT = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
                if (isTaskOfT)
                {
                    typeArg = type.GetGenericArguments()[0];
                    if (typeArg != typeof(void) && typeArg.FullName != "System.Threading.Tasks.VoidTaskResult")
                        return true;
                }
                type = type.BaseType;
            }

            typeArg = null;
            return false;
        }

        private static bool IsValueTask(Type type)
        {
            return typeof(ValueTask).IsAssignableFrom(type) || IsValueTaskOfT(type, out _);
        }

        private static bool IsValueTaskOfT(Type type, out Type typeArg)
        {
            typeArg = null;
            var isTaskOfT = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTask<>);
            if (isTaskOfT)
            {
                typeArg = type.GetGenericArguments()[0];
            }
            return isTaskOfT;
        }

        private static bool IsAwaitableOfT(Type type, out Type typeArg)
        {
            return IsTaskOfT(type, out typeArg) || IsValueTaskOfT(type, out typeArg);
        }

        public static bool IsAwaitable(Type type)
        {
            return IsTask(type) || IsValueTask(type);
        }

        public static bool IsAwaitableAsTask(object obj, out Task task)
        {
            if (obj is Task taskObject)
            {
                task = taskObject;
                return true;
            }
            if (obj is ValueTask valueTask)
            {
                task = valueTask.AsTask();
                return true;
            }
            if (obj != null && IsValueTaskOfT(obj.GetType(), out _))
            {
                // unfortunately there is no base class/interface of the Value<T> types, so we can only call the "AsTask" method via reflection
                var asTaskMethod = obj.GetType().GetMethod(nameof(ValueTask<object>.AsTask));
                task = (Task)asTaskMethod!.Invoke(obj, Array.Empty<object>());
                return true;
            }

            task = null;
            return false;
        }

        public static IBindingType GetAwaitableReturnType(this IBindingMethod bindingMethod)
        {
            var returnType = bindingMethod.ReturnType;

            if (returnType is RuntimeBindingType runtimeReturnType &&
                IsAwaitableOfT(runtimeReturnType.Type, out var typeArg))
            {
                return new RuntimeBindingType(typeArg);
            }

            return returnType;
        }

        public static bool IsAsyncVoid(MethodInfo methodInfo)
        {
            return 
                methodInfo.ReturnType == typeof(void) &&
                methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>() != null;
        }

        public static bool IsAsyncVoid(IBindingMethod bindingMethod)
        {
            if (bindingMethod is RuntimeBindingMethod runtimeBindingMethod) 
                return IsAsyncVoid(runtimeBindingMethod.MethodInfo);
            return false;
        }

        public static Task<object> ConvertToTaskOfObject(Task task)
        {
            if (task.GetType() == typeof(Task))
                return ConvertTaskOfT(task, false);
            if (task is Task<object> taskOfObj)
                return taskOfObj;
            if (IsTaskOfT(task.GetType(), out _))
                return ConvertTaskOfT(task, true);
            return ConvertTaskOfT(task, false);
        }

        // We are allowed to have async method here, because the synchronous part of the task (that might have changed the ExecutionContext)
        // has been executed already.
        private static async Task<object> ConvertTaskOfT(Task task, bool getValue)
        {
            await task;
            return getValue ? task.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(task) : null;
        }
    }
}
