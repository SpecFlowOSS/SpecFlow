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
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    public abstract class MethodBinding
    {
        private readonly ErrorProvider errorProvider;

        protected MethodBinding(MethodInfo method)
        {
            errorProvider = ObjectContainer.ErrorProvider;

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
            if (method.ReturnType == typeof (void))
                return CreateActionDelegate(method);
            else
                return CreateFuncDelegate(method);
        }

        private Delegate CreateActionDelegate(MethodInfo method)
        {
            List<ParameterExpression> parameters = new List<ParameterExpression>();
            foreach (ParameterInfo parameterInfo in method.GetParameters())
            {
                parameters.Add(Expression.Parameter(parameterInfo.ParameterType, parameterInfo.Name));
            }

            LambdaExpression lambda;

            if (method.IsStatic)
            {
                lambda = Expression.Lambda(
                    GetActionType(parameters.Select(p => p.Type).ToArray()),
                    Expression.Call(method, parameters.Cast<Expression>().ToArray()),
                    parameters.ToArray());
            }
            else
            {
                Type bindingType = method.DeclaringType;
                Expression<Func<object>> getInstanceExpression =
                    () => ScenarioContext.Current.GetBindingInstance(bindingType);

                lambda = Expression.Lambda(
                    GetActionType(parameters.Select(p => p.Type).ToArray()),
                    Expression.Call(
                        Expression.Convert(getInstanceExpression.Body, bindingType),
                        method,
                        parameters.Cast<Expression>().ToArray()),
                    parameters.ToArray());
            }


            return lambda.Compile();
        }

        private Delegate CreateFuncDelegate(MethodInfo method)
        {
            List<ParameterExpression> parameters = new List<ParameterExpression>();
            foreach (ParameterInfo parameterInfo in method.GetParameters())
            {
                parameters.Add(Expression.Parameter(parameterInfo.ParameterType, parameterInfo.Name));
            }

            LambdaExpression lambda;

            if (method.IsStatic)
            {
                lambda = Expression.Lambda(
                    GetFuncType(parameters.Select(p => p.Type).ToArray(), method.ReturnType),
                    Expression.Call(method, parameters.Cast<Expression>().ToArray()),
                    parameters.ToArray());
            }
            else
            {
                Type bindingType = method.DeclaringType;
                Expression<Func<object>> getInstanceExpression =
                    () => ScenarioContext.Current.GetBindingInstance(bindingType);

                lambda = Expression.Lambda(
                    GetFuncType(parameters.Select(p => p.Type).ToArray(), method.ReturnType),
                    Expression.Call(
                        Expression.Convert(getInstanceExpression.Body, bindingType),
                        method,
                        parameters.Cast<Expression>().ToArray()),
                    parameters.ToArray());
            }


            return lambda.Compile();
        }

        public object InvokeAction(object[] arguments, ITestTracer testTracer)
        {
            TimeSpan duration;
            return InvokeAction(arguments, testTracer, out duration);
        }

        public object InvokeAction(object[] arguments, ITestTracer testTracer, out TimeSpan duration)
        {
            try
            {
                object result;
                Stopwatch stopwatch = new Stopwatch();
                using (CreateCultureInfoScope())
                {
                    stopwatch.Start();
                    result = BindingAction.DynamicInvoke(arguments);
                    stopwatch.Stop();
                }

                if (RuntimeConfiguration.Current.TraceTimings && stopwatch.Elapsed >= RuntimeConfiguration.Current.MinTracedDuration)
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

        private CultureInfoScope CreateCultureInfoScope()
        {
            var cultureInfo = CultureInfo.CurrentCulture;
            if (FeatureContext.Current != null)
            {
                cultureInfo = FeatureContext.Current.BindingCulture;
            }
            return new CultureInfoScope(cultureInfo);
        }

        #region extended action types
        static readonly Type[] actionTypes = new Type[] { typeof(Action), typeof(Action<>), typeof(Action<,>), typeof(Action<,,>), typeof(Action<,,,>), 
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
        static readonly Type[] funcTypes = new Type[] { typeof(Func<>), typeof(Func<,>), typeof(Func<,,>), typeof(Func<,,,>), 
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
            var genericTypeArgs = typeArgs.Concat(new Type[] {resultType}).ToArray();
            return funcTypes[typeArgs.Length].MakeGenericType(genericTypeArgs);
        }
        #endregion
    }
}