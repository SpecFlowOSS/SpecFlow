using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.ErrorHandling;

namespace TechTalk.SpecFlow
{
    internal enum BindingEvent
    {
        TestRunStart,
        TestRunEnd,
        FeatureStart,
        FeatureEnd,
        ScenarioStart,
        ScenarioEnd,
        BlockStart,
        BlockEnd,
        StepStart,
        StepEnd
    }

    internal class BindingRegistry : IEnumerable<StepBinding>
    {
        private readonly ErrorProvider errorProvider;

        private readonly List<StepBinding> stepBindings = new List<StepBinding>();
        private readonly Dictionary<BindingEvent, List<EventBinding>> eventBindings = new Dictionary<BindingEvent, List<EventBinding>>();

        public BindingRegistry()
        {
            this.errorProvider = ObjectContainer.ErrorProvider;
        }

        public void BuildBindingsFromAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                BindingAttribute bindingAttr = (BindingAttribute)Attribute.GetCustomAttribute(type, typeof (BindingAttribute));
                if (bindingAttr == null)
                    continue;

                BuildBindingsFromType(type);
            }
        }

        private void BuildBindingsFromType(Type type)
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var scenarioStepAttrs = Attribute.GetCustomAttributes(method, typeof(ScenarioStepAttribute));
                if (scenarioStepAttrs != null)
                    foreach (ScenarioStepAttribute scenarioStepAttr in scenarioStepAttrs)
                    {
                        BuildStepBindingFromMethod(method, scenarioStepAttr);
                    }

                var bindingEventAttrs = Attribute.GetCustomAttributes(method, typeof(BindingEventAttribute));
                if (bindingEventAttrs != null)
                    foreach (BindingEventAttribute bindingEventAttr in bindingEventAttrs)
                    {
                        BuildEventBindingFromMethod(method, bindingEventAttr);
                    }
            }
        }

        public List<EventBinding> GetEvents(BindingEvent bindingEvent)
        {
            List<EventBinding> list;
            if (!eventBindings.TryGetValue(bindingEvent, out list))
            {
                list = new List<EventBinding>();
                eventBindings.Add(bindingEvent, list);
            }

            return list;
        }

        private void BuildEventBindingFromMethod(MethodInfo method, BindingEventAttribute bindingEventAttr)
        {
            CheckEventBindingMethod(bindingEventAttr.Event, method);

            Delegate bindingAction = CreateBindingAction(method);
            var eventBinding = new EventBinding(bindingAction, bindingEventAttr.Tags, method);

            GetEvents(bindingEventAttr.Event).Add(eventBinding);
        }

        private void CheckEventBindingMethod(BindingEvent bindingEvent, MethodInfo method)
        {
            if (!IsScenarioSpecificEvent(bindingEvent) &&
                !method.IsStatic)
                throw errorProvider.GetNonStaticEventError(method);

            //TODO: check parameters, etc.
        }

        private bool IsScenarioSpecificEvent(BindingEvent bindingEvent)
        {
            return
                bindingEvent == BindingEvent.ScenarioStart ||
                bindingEvent == BindingEvent.ScenarioEnd ||
                bindingEvent == BindingEvent.BlockStart ||
                bindingEvent == BindingEvent.BlockEnd ||
                bindingEvent == BindingEvent.StepStart ||
                bindingEvent == BindingEvent.StepEnd;
        }

        private void BuildStepBindingFromMethod(MethodInfo method, ScenarioStepAttribute scenarioStepAttr)
        {
            CheckStepBindingMethod(method);

            Regex regex = new Regex("^" + scenarioStepAttr.Regex + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            Delegate bindingAction = CreateBindingAction(method);
            var parameterTypes = method.GetParameters().Select(pi => pi.ParameterType).ToArray();
            StepBinding stepBinding = new StepBinding(scenarioStepAttr.Type, regex, bindingAction, parameterTypes, method);

            stepBindings.Add(stepBinding);
        }

        private void CheckStepBindingMethod(MethodInfo method)
        {
            //TODO: check parameters, etc.
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

        private Delegate CreateBindingAction(MethodInfo method)
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

        public IEnumerator<StepBinding> GetEnumerator()
        {
            return stepBindings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}