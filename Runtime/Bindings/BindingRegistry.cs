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

    public interface IBindingRegistry : IEnumerable<StepDefinitionBinding>
    {
        void BuildBindingsFromAssembly(Assembly assembly);
        List<IHookBinding> GetEvents(BindingEvent bindingEvent);
        ICollection<StepTransformationBinding> StepTransformations { get; }
    }

    internal class BindingRegistry : IBindingRegistry
    {
        private readonly IErrorProvider errorProvider;
        private readonly IBindingFactory bindingFactory;

        private readonly List<StepDefinitionBinding> stepBindings = new List<StepDefinitionBinding>();
        private readonly List<StepTransformationBinding> stepTransformations = new List<StepTransformationBinding>();
        private readonly Dictionary<BindingEvent, List<IHookBinding>> eventBindings = new Dictionary<BindingEvent, List<IHookBinding>>();

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
                var scenarioStepAttrs = Attribute.GetCustomAttributes(method, typeof(StepDefinitionBaseAttribute));
                if (scenarioStepAttrs != null)
                    foreach (StepDefinitionBaseAttribute scenarioStepAttr in scenarioStepAttrs)
                    {
                        BuildStepBindingFromMethod(method, scenarioStepAttr);
                    }

                var bindingEventAttrs = Attribute.GetCustomAttributes(method, typeof(HookAttribute));
                if (bindingEventAttrs != null)
                    foreach (HookAttribute bindingEventAttr in bindingEventAttrs)
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

        public List<IHookBinding> GetEvents(BindingEvent bindingEvent)
        {
            List<IHookBinding> list;
            if (!eventBindings.TryGetValue(bindingEvent, out list))
            {
                list = new List<IHookBinding>();
                eventBindings.Add(bindingEvent, list);
            }

            return list;
        }

        public ICollection<StepTransformationBinding> StepTransformations 
        { 
            get { return stepTransformations; }
        }

        private void BuildEventBindingFromMethod(MethodInfo method, HookAttribute hookAttr)
        {
            CheckEventBindingMethod(hookAttr.Event, method);

            ApplyForScope(method,
                          scope =>
                              {
                                  var eventBinding = bindingFactory.CreateEventBinding(method, scope);
                                  GetEvents(hookAttr.Event).Add(eventBinding);
                              },
                          hookAttr.Tags == null
                              ? null
                              : hookAttr.Tags.Select(tag => new ScopeAttribute { Tag = tag })
                );
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

        private void ApplyForScope(MethodInfo method, Action<BindingScope> action, IEnumerable<ScopeAttribute> additionalScopeAttrs = null)
        {
            var scopeAttrs =
                Attribute.GetCustomAttributes(method.ReflectedType, typeof(ScopeAttribute)).Concat(
                Attribute.GetCustomAttributes(method, typeof(ScopeAttribute))).Cast<ScopeAttribute>();

            if (additionalScopeAttrs != null)
                scopeAttrs = scopeAttrs.Concat(additionalScopeAttrs);

            if (scopeAttrs.Any())
            {
                foreach (var scopeAttr in scopeAttrs)
                {
                    action(CreateScope(scopeAttr));
                }
            }
            else
            {
                action(null);
            }
        }

        private void BuildStepBindingFromMethod(MethodInfo method, StepDefinitionBaseAttribute stepDefinitionBaseAttr)
        {
            CheckStepBindingMethod(method);
            ApplyForScope(method, scope => AddStepBinding(method, stepDefinitionBaseAttr, scope));
        }

        private BindingScope CreateScope(ScopeAttribute scopeAttr)
        {
            if (scopeAttr.Tag == null && scopeAttr.Feature == null && scopeAttr.Scenario == null)
                return null;

            return new BindingScope(scopeAttr.Tag, scopeAttr.Feature, scopeAttr.Scenario);
        }

        private void AddStepBinding(MethodInfo method, StepDefinitionBaseAttribute stepDefinitionBaseAttr, BindingScope stepScope)
        {
            foreach (var bindingType in stepDefinitionBaseAttr.Types)
            {
                var stepBinding = bindingFactory.CreateStepBinding(bindingType, stepDefinitionBaseAttr.Regex, method, stepScope);
                stepBindings.Add(stepBinding);
            }
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

        public IEnumerator<StepDefinitionBinding> GetEnumerator()
        {
            return stepBindings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}