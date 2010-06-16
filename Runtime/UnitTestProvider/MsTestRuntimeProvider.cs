using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TechTalk.SpecFlow.UnitTestProvider
{
    internal static class UnitTestRuntimeProviderHelper
    {
        public static Action<string> GetAssertMethod(string assemblyName, string typeName, string methodName)
        {
            Assembly msTestAssembly = Assembly.Load(assemblyName);
            Type assertType = msTestAssembly.GetType(typeName, true);

            MethodInfo method = assertType.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.Static, null,
                new Type[] { typeof(string) }, null);
            if (method == null)
                throw new SpecFlowException("Assert method not found: " + methodName);

            List<ParameterExpression> parameters = new List<ParameterExpression>();
            foreach (ParameterInfo parameterInfo in method.GetParameters())
            {
                parameters.Add(Expression.Parameter(parameterInfo.ParameterType, parameterInfo.Name));
            }
            var lambda = Expression.Lambda<Action<string>>(
                    Expression.Call(method, parameters.Cast<Expression>().ToArray()),
                    parameters.ToArray());

            return lambda.Compile();
        }

    }

    public class MsTestRuntimeProvider : IUnitTestRuntimeProvider
    {
        private const string MSTEST_ASSEMBLY = "Microsoft.VisualStudio.QualityTools.UnitTestFramework";

        private const string ASSERT_TYPE = "Microsoft.VisualStudio.TestTools.UnitTesting.Assert";

        Action<string> assertInconclusive = null;
        
        public virtual string AssemblyName
        {
            get { return MSTEST_ASSEMBLY; }
        }

        public void TestInconclusive(string message)
        {
            if (assertInconclusive == null)
            {
                assertInconclusive = UnitTestRuntimeProviderHelper.GetAssertMethod(AssemblyName, ASSERT_TYPE, "Inconclusive");
            }

            assertInconclusive(message);
        }

        public void TestIgnore(string message)
        {
            TestInconclusive(message); // there is no dynamic "Ignore" in mstest
        }

        public bool DelayedFixtureTearDown
        {
            get { return true; }
        }

        
    }
}