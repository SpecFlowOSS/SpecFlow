using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.ErrorHandling;

namespace TechTalk.SpecFlow.Bindings
{
    public enum BindingEvent
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

    public interface IBindingRegistry : IEnumerable<StepBinding>
    {
        void BuildBindingsFromAssembly(Assembly assembly);
        List<EventBinding> GetEvents(BindingEvent bindingEvent);
        ICollection<StepTransformationBinding> StepTransformations { get; }
    }

    internal class BindingRegistry : IBindingRegistry
    {
        private readonly IErrorProvider errorProvider;
        private readonly IBindingFactory bindingFactory;

        private readonly List<StepBinding> stepBindings = new List<StepBinding>();
        private readonly List<StepTransformationBinding> stepTransformations = new List<StepTransformationBinding>();
        private readonly Dictionary<BindingEvent, List<EventBinding>> eventBindings = new Dictionary<BindingEvent, List<EventBinding>>();

        public BindingRegistry(IErrorProvider errorProvider, IBindingFactory bindingFactory)
        {
            this.errorProvider = errorProvider;
            this.bindingFactory = bindingFactory;
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

        internal void BuildBindingsFromType(Type type)
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

                var stepTransformationAttrs = Attribute.GetCustomAttributes(method, typeof(StepArgumentTransformationAttribute));
                if (stepTransformationAttrs != null)
                    foreach (StepArgumentTransformationAttribute stepTransformationAttr in stepTransformationAttrs)
                    {
                        BuildStepTransformationFromMethod(method, stepTransformationAttr);
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

        public ICollection<StepTransformationBinding> StepTransformations 
        { 
            get { return stepTransformations; }
        }

        private void BuildEventBindingFromMethod(MethodInfo method, BindingEventAttribute bindingEventAttr)
        {
            CheckEventBindingMethod(bindingEventAttr.Event, method);

            var eventBinding = bindingFactory.CreateEventBinding(bindingEventAttr.Tags, method);

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

            var scopeAttrs = 
                Attribute.GetCustomAttributes(method.ReflectedType, typeof(StepScopeAttribute)).Concat(
                Attribute.GetCustomAttributes(method, typeof(StepScopeAttribute)));

            if (scopeAttrs.Any())
            {
                foreach (StepScopeAttribute scopeAttr in scopeAttrs)
                {
                    AddStepBinding(method, scenarioStepAttr, CreateScope(scopeAttr));
                }
            }
            else
            {
                AddStepBinding(method, scenarioStepAttr, null);
            }
        }

        private BindingScope CreateScope(StepScopeAttribute scopeAttr)
        {
            if (scopeAttr.Tag == null && scopeAttr.Feature == null && scopeAttr.Scenario == null)
                return null;

            return new BindingScope(scopeAttr.Tag, scopeAttr.Feature, scopeAttr.Scenario);
        }

        private void AddStepBinding(MethodInfo method, ScenarioStepAttribute scenarioStepAttr, BindingScope stepScope)
        {
            var stepBinding = bindingFactory.CreateStepBinding(scenarioStepAttr.Type, scenarioStepAttr.Regex, method, stepScope);
            stepBindings.Add(stepBinding);
        }

        private void BuildStepTransformationFromMethod(MethodInfo method, StepArgumentTransformationAttribute argumentTransformationAttribute)
        {
            var stepTransformationBinding = bindingFactory.CreateStepArgumentTransformation(argumentTransformationAttribute.Regex, method);

            stepTransformations.Add(stepTransformationBinding);
        }

        private void CheckStepBindingMethod(MethodInfo method)
        {
            //TODO: check parameters, etc.
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