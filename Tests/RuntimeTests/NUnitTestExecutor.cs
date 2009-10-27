using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace TechTalk.SpecFlow.RuntimeTests
{
    class NUnitTestExecutor
    {
        public static void ExecuteNUnitTests(object test)
        {
            // fixture setup
            ExecuteWithAttribute(test, typeof(TestFixtureSetUpAttribute));

            foreach (var testMethod in GetMethodsWithAttribute(test, typeof(TestAttribute)))
            {
                // test setup
                ExecuteWithAttribute(test, typeof(SetUpAttribute));

                InvokeMethod(test, testMethod);

                // test teardown
                ExecuteWithAttribute(test, typeof(TearDownAttribute));
            }

            // fixture teardown
            ExecuteWithAttribute(test, typeof(TestFixtureTearDownAttribute));
        }

        private static void InvokeMethod(object test, MethodInfo testMethod)
        {
            try
            {
                testMethod.Invoke(test, null);
            }
            catch (TargetInvocationException invEx)
            {
                var ex = invEx.InnerException;
                PreserveStackTrace(ex);
                throw ex;
            }
        }

        internal static void PreserveStackTrace(Exception ex)
        {
            typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ex, new object[0]);
        }

        private static void ExecuteWithAttribute(object test, Type attributeType)
        {
            foreach (var methodInfo in GetMethodsWithAttribute(test, attributeType))
            {
                InvokeMethod(test, methodInfo);
            }
        }

        private static IEnumerable<MethodInfo> GetMethodsWithAttribute(object test, Type attributeType)
        {
            foreach (var methodInfo in test.GetType().GetMethods())
            {
                var attr = Attribute.GetCustomAttribute(methodInfo, attributeType, true);
                if (attr == null)
                    continue;

                yield return methodInfo;
            }
        }
    }
}
