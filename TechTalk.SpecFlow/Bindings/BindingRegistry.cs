using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Bindings
{
    public class BindingRegistry : IBindingRegistry
    {
        private readonly List<IStepDefinitionBinding> stepDefinitions = new List<IStepDefinitionBinding>();
        private readonly List<IStepArgumentTransformationBinding> stepArgumentTransformations = new List<IStepArgumentTransformationBinding>();
        private readonly Dictionary<HookType, List<IHookBinding>> hooks = new Dictionary<HookType, List<IHookBinding>>();

        public bool Ready { get; set; }

        public IEnumerable<IStepDefinitionBinding> GetStepDefinitions()
        {
            return stepDefinitions;
        }

        public IEnumerable<IStepDefinitionBinding> GetConsideredStepDefinitions(StepDefinitionType stepDefinitionType, string stepText)
        {
            //TODO: later optimize to return step definitions that has a chance to match to stepText
            return stepDefinitions.Where(sd => sd.StepDefinitionType == stepDefinitionType);
        }

        public virtual IEnumerable<IHookBinding> GetHooks()
        {
            return hooks.Values.SelectMany(hookList => hookList);
        }

        public virtual IEnumerable<IHookBinding> GetHooks(HookType bindingEvent)
        {
            return GetHookList(bindingEvent);
        }

        private IEnumerable<IHookBinding> GetHookList(HookType bindingEvent)
        {
            List<IHookBinding> list;
            if (hooks.TryGetValue(bindingEvent, out list))
                return list;

            return Enumerable.Empty<IHookBinding>();
        }

        public virtual IEnumerable<IStepArgumentTransformationBinding> GetStepTransformations()
        {
            return stepArgumentTransformations;
        }

        public virtual void RegisterStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding)
        {
            stepDefinitions.Add(stepDefinitionBinding);
        }

        private List<IHookBinding> GetHookListForRegister(HookType bindingEvent)
        {
            List<IHookBinding> list;
            if (!hooks.TryGetValue(bindingEvent, out list))
            {
                list = new List<IHookBinding>();
                hooks.Add(bindingEvent, list);
            }

            return list;
        }

        public virtual void RegisterHookBinding(IHookBinding hookBinding)
        {
            List<IHookBinding> hookRegistry = GetHookListForRegister(hookBinding.HookType);

            if (!hookRegistry.Contains(hookBinding))
                hookRegistry.Add(hookBinding);
        }

        public virtual void RegisterStepArgumentTransformationBinding(IStepArgumentTransformationBinding stepArgumentTransformationBinding)
        {
            stepArgumentTransformations.Add(stepArgumentTransformationBinding);
        }
    }
}