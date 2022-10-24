using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    internal static class AsyncMethodHelper
    {
        private static readonly IBindingType TaskOfT = new RuntimeBindingType(typeof(Task<>));

        private static bool IsTaskOfT(Type type, out Type typeArg)
        {
            typeArg = null;
            var isTaskOfT = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
            if (isTaskOfT)
            {
                typeArg = type.GetGenericArguments()[0];
            }
            return isTaskOfT;
        }

        private static bool IsValueTaskOfT(Type type, out Type typeArg)
        {
            typeArg = null;
#if NETFRAMEWORK
            return false;
#else
            var isTaskOfT = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTask<>);
            if (isTaskOfT)
            {
                typeArg = type.GetGenericArguments()[0];
            }
            return isTaskOfT;
#endif
        }

        private static bool IsAwaitableOfT(Type type, out Type typeArg)
        {
            return IsTaskOfT(type, out typeArg) || IsValueTaskOfT(type, out typeArg);
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
    }
}
