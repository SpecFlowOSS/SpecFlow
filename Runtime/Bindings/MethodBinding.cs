using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    public abstract class MethodBinding
    {
        private readonly RuntimeConfiguration runtimeConfiguration;
        private readonly IErrorProvider errorProvider;

        protected MethodBinding(RuntimeConfiguration runtimeConfiguration, IErrorProvider errorProvider, MethodInfo method)
        {
            this.runtimeConfiguration = runtimeConfiguration;
            this.errorProvider = errorProvider;

            BindingAction = CreateMethodDelegate(method);
            MethodInfo = method;
            ParameterTypes = method.GetParameters().Select(pi => pi.ParameterType).ToArray();
            ReturnType = method.ReturnType;
        }

        public Delegate BindingAction { get; protected set; }
        public MethodInfo MethodInfo { get; private set; }
        public Type[] ParameterTypes { get; private set; }
        public Type ReturnType { get; private set; }

        protected Delegate CreateMethodDelegate(MethodInfo method)
        {
            List<ParameterExpression> parameters = new List<ParameterExpression>();
            parameters.Add(Expression.Parameter(typeof(IContextManager), "__contextManager"));
            parameters.AddRange(method.GetParameters().Select(parameterInfo => Expression.Parameter(parameterInfo.ParameterType, parameterInfo.Name)));
            var methodArguments = parameters.Skip(1).Cast<Expression>().ToArray();
            var contextManagerArg = parameters[0];

            LambdaExpression lambda;

            var delegateType = GetDelegateType(parameters.Select(p => p.Type).ToArray(), method.ReturnType);

            if (method.IsStatic)
            {
                lambda = Expression.Lambda(
                    delegateType,
                    Expression.Call(method, methodArguments),
                    parameters.ToArray());
            }
            else
            {
                var getInstanceMethod = ExpressionMemberAccessor.GetMethodInfo((ScenarioContext sc) => sc.GetBindingInstance(null));
                Debug.Assert(getInstanceMethod != null, "GetBindingInstance not found");
                var scenarioContextProperty = ExpressionMemberAccessor.GetPropertyInfo((IContextManager cm) => cm.ScenarioContext);
                Debug.Assert(scenarioContextProperty != null, "ScenarioContext not found");

                lambda = Expression.Lambda(
                    delegateType,
                    Expression.Call(
                        Expression.Convert(
                            Expression.Call(
                                Expression.Property(
                                    contextManagerArg,
                                    scenarioContextProperty), 
                                getInstanceMethod,
                                Expression.Constant(method.ReflectedType)),
                            method.ReflectedType), 
                        method, 
                        methodArguments),
                    parameters.ToArray());
            }

#if WINDOWS_PHONE
            return ExpressionCompiler.ExpressionCompiler.Compile(lambda);
#else
            return lambda.Compile();
#endif
        }

        public object InvokeAction(IContextManager contextManager, object[] arguments, ITestTracer testTracer)
        {
            TimeSpan duration;
            return InvokeAction(contextManager, arguments, testTracer, out duration);
        }

        public object InvokeAction(IContextManager contextManager, object[] arguments, ITestTracer testTracer, out TimeSpan duration)
        {
            try
            {
                object result;
                Stopwatch stopwatch = new Stopwatch();
                using (CreateCultureInfoScope(contextManager))
                {
                    stopwatch.Start();
                    object[] invokeArgs = new object[arguments == null ? 1 : arguments.Length + 1];
                    if (arguments != null)
                        Array.Copy(arguments, 0, invokeArgs, 1, arguments.Length);
                    invokeArgs[0] = contextManager;
                    result = BindingAction.DynamicInvoke(invokeArgs);
                    stopwatch.Stop();
                }

                if (runtimeConfiguration.TraceTimings && stopwatch.Elapsed >= runtimeConfiguration.MinTracedDuration)
                {
                    testTracer.TraceDuration(stopwatch.Elapsed, MethodInfo, arguments);
                }

                duration = stopwatch.Elapsed;
                return result;
            }
            catch (ArgumentException ex)
            {
                throw errorProvider.GetCallError(MethodInfo, ex);
            }
            catch (TargetInvocationException invEx)
            {
                var ex = invEx.InnerException;
                ex = ex.PreserveStackTrace(errorProvider.GetMethodText(MethodInfo));
                throw ex;
            }
        }

        private CultureInfoScope CreateCultureInfoScope(IContextManager contextManager)
        {
            var cultureInfo = CultureInfo.CurrentCulture;
            if (contextManager.FeatureContext != null)
            {
                cultureInfo = contextManager.FeatureContext.BindingCulture;
            }
            return new CultureInfoScope(cultureInfo);
        }

        #region extended action types
        static readonly Type[] actionTypes = new[] { typeof(Action), typeof(Action<>), typeof(Action<,>), typeof(Action<,,>), typeof(Action<,,,>), 
                                                          typeof(ExtendedAction<,,,,>), typeof(ExtendedAction<,,,,,>), typeof(ExtendedAction<,,,,,,>), typeof(ExtendedAction<,,,,,,,>), typeof(ExtendedAction<,,,,,,,,>), typeof(ExtendedAction<,,,,,,,,,>)
                                                        };

        public delegate void ExtendedAction<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        public delegate void ExtendedAction<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        public delegate void ExtendedAction<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
        public delegate void ExtendedAction<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
        public delegate void ExtendedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
        public delegate void ExtendedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);

        private Type GetActionType(Type[] typeArgs)
        {
            if (typeArgs.Length >= actionTypes.Length)
            {
                throw errorProvider.GetTooManyBindingParamError(actionTypes.Length - 1);
            }
            if (typeArgs.Length == 0)
            {
                return actionTypes[typeArgs.Length];
            }
            return actionTypes[typeArgs.Length].MakeGenericType(typeArgs);
        }
        #endregion   
        
        #region extended func types
        static readonly Type[] funcTypes = new[] { typeof(Func<>), typeof(Func<,>), typeof(Func<,,>), typeof(Func<,,,>), 
                                                        typeof(Func<,,,,>), typeof(ExtendedFunc<,,,,,>), typeof(ExtendedFunc<,,,,,,>), typeof(ExtendedFunc<,,,,,,,>), typeof(ExtendedFunc<,,,,,,,,>), typeof(ExtendedFunc<,,,,,,,,,>)
                                                      };

        public delegate TResult ExtendedFunc<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        public delegate TResult ExtendedFunc<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        public delegate TResult ExtendedFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
        public delegate TResult ExtendedFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
        public delegate TResult ExtendedFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
        public delegate TResult ExtendedFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);

        private Type GetFuncType(Type[] typeArgs, Type resultType)
        {
            if (typeArgs.Length >= actionTypes.Length)
            {
                throw errorProvider.GetTooManyBindingParamError(actionTypes.Length - 1);
            }
            if (typeArgs.Length == 0)
            {
                return funcTypes[0].MakeGenericType(resultType);
            }
            var genericTypeArgs = typeArgs.Concat(new[] {resultType}).ToArray();
            return funcTypes[typeArgs.Length].MakeGenericType(genericTypeArgs);
        }

        private Type GetDelegateType(Type[] typeArgs, Type resultType)
        {
            if (resultType == typeof(void))
                return GetActionType(typeArgs);

            return GetFuncType(typeArgs, resultType);
        }

        #endregion
    }
}