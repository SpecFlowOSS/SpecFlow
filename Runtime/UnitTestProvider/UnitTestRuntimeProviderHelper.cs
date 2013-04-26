using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TechTalk.SpecFlow.UnitTestProvider
{
    internal static class UnitTestRuntimeProviderHelper
    {
        public static Action<string, object[]> GetAssertMethodWithFormattedMessage(string assemblyName, string typeName, string methodName)
        {
#if WINRT
            Assembly msTestAssembly = Assembly.Load(new AssemblyName(assemblyName));
            Type assertType = msTestAssembly.GetType(typeName);

            MethodInfo method =
                assertType.GetRuntimeMethods().FirstOrDefault(
                    m => m.IsPublic && m.IsStatic && m.GetParameters().Count() == 2 && m.GetParameters()[0].ParameterType == typeof(string) && m.GetParameters()[1].ParameterType == typeof(object[]));
#else
            Assembly msTestAssembly = Assembly.Load(assemblyName);
            Type assertType = msTestAssembly.GetType(typeName, true);

            MethodInfo method = assertType.GetMethod(methodName,
                                                     BindingFlags.Public | BindingFlags.Static, null,
                                                     new Type[] { typeof(string), typeof(object[]) }, null);
#endif

            if (method == null)
                throw new SpecFlowException("Assert method not found: " + methodName);

            List<ParameterExpression> parameters = new List<ParameterExpression>();
            foreach (ParameterInfo parameterInfo in method.GetParameters())
            {
                parameters.Add(Expression.Parameter(parameterInfo.ParameterType, parameterInfo.Name));
            }
            var lambda = Expression.Lambda<Action<string, object[]>>(
                Expression.Call(method, parameters.Cast<Expression>().ToArray()),
                parameters.ToArray());

#if WINDOWS_PHONE
						return ExpressionCompiler.ExpressionCompiler.Compile(lambda) as Action<string, object[]>;
#else
            return lambda.Compile();
#endif
        }

    }
}