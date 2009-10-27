using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

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
        private readonly List<StepBinding> stepBindings = new List<StepBinding>();
        private readonly Dictionary<BindingEvent, List<EventBinding>> eventBindings = new Dictionary<BindingEvent, List<EventBinding>>();

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
            CheckEventBindingMethod(method);

            Delegate bindingAction = CreateBindingAction(method);
            var eventBinding = new EventBinding(bindingAction, bindingEventAttr.Tags, method);

            GetEvents(bindingEventAttr.Event).Add(eventBinding);
        }

        private void CheckEventBindingMethod(MethodInfo method)
        {
            if (!method.IsStatic)
                throw new Exception("The event binding method must be static! " + method.ToString());

            //TODO: check parameters, etc.
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
//            if (!method.IsStatic)
//                throw new Exception("The binding method must be static! " + method.ToString());

            //TODO: check parameters, etc.
        }

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
                    Expression.Call(method, parameters.Cast<Expression>().ToArray()),
                    parameters.ToArray());
            }
            else
            {
                Type bindingType = method.DeclaringType;
                Expression<Func<object>> getInstanceExpression =
                    () => ScenarioContext.Current.GetBindingInstance(bindingType);

                lambda = Expression.Lambda(
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