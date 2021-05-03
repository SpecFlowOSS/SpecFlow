using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingRegistry
    {
        bool Ready { get; set; }

        IEnumerable<IStepDefinitionBinding> GetStepDefinitions();
        IEnumerable<IHookBinding> GetHooks();
        IEnumerable<IStepDefinitionBinding> GetConsideredStepDefinitions(StepDefinitionType stepDefinitionType, string stepText = null);
        IEnumerable<IHookBinding> GetHooks(HookType bindingEvent);
        IEnumerable<IStepArgumentTransformationBinding> GetStepTransformations();

        void RegisterStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding);
        void RegisterHookBinding(IHookBinding hookBinding);
        void RegisterStepArgumentTransformationBinding(IStepArgumentTransformationBinding stepArgumentTransformationBinding);
    }

    public class BindingRegistry : IBindingRegistry
    {
        private readonly List<IStepDefinitionBinding> stepDefinitions = new List<IStepDefinitionBinding>();
        private readonly List<IStepArgumentTransformationBinding> stepArgumentTransformations = new List<IStepArgumentTransformationBinding>();
        private readonly Dictionary<HookType, HashSet<IHookBinding>> hooks = new Dictionary<HookType, HashSet<IHookBinding>>();

        public bool Ready { get; set; }

        public IEnumerable<IStepDefinitionBinding> GetStepDefinitions()
        {
            return stepDefinitions;
        }

        public IEnumerable<IStepDefinitionBinding> GetConsideredStepDefinitions(StepDefinitionType stepDefinitionType, string stepText)
        {
            //TODO: later optimize to return step definitions that has a chance to match to stepText
            foreach (IStepDefinitionBinding sd in stepDefinitions)
            {
                if (sd.StepDefinitionType == stepDefinitionType)
                {
                    yield return sd;
                }
            }
        }

        public virtual IEnumerable<IHookBinding> GetHooks()
        {
            foreach (var hookSet in hooks.Values)
            {
                foreach (IHookBinding binding in hookSet)
                {
                    yield return binding;
                }
            }
        }

        public virtual IEnumerable<IHookBinding> GetHooks(HookType bindingEvent)
        {
            if (hooks.TryGetValue(bindingEvent, out var list))
            {
                return list;
            }

            return Array.Empty<IHookBinding>();
        }

        public virtual IEnumerable<IStepArgumentTransformationBinding> GetStepTransformations()
        {
            return stepArgumentTransformations;
        }

        public virtual void RegisterStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding)
        {
            stepDefinitions.Add(stepDefinitionBinding);
        }

        public virtual void RegisterHookBinding(IHookBinding hookBinding)
        {
            if (hooks.TryGetValue(hookBinding.HookType, out var set))
            {
                set.Add(hookBinding);
            }
            else
            {
#if NETCOREAPP2_1_OR_GREATER
                set = new HashSet<IHookBinding>(1)
#else
                set = new HashSet<IHookBinding>
#endif
                {
                    hookBinding
                };
                hooks.Add(hookBinding.HookType, set);
            }
        }

        public virtual void RegisterStepArgumentTransformationBinding(IStepArgumentTransformationBinding stepArgumentTransformationBinding)
        {
            stepArgumentTransformations.Add(stepArgumentTransformationBinding);
        }
    }
}