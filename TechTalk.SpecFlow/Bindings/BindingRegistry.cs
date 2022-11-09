using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Bindings
{
    public class BindingRegistry : IBindingRegistry
    {
        private readonly List<IStepDefinitionBinding> _stepDefinitions = new();
        private readonly List<IStepArgumentTransformationBinding> _stepArgumentTransformations = new();
        private readonly Dictionary<HookType, List<IHookBinding>> _hooks = new();
        private readonly List<BindingError> _genericBindingErrors = new();

        public bool Ready { get; set; }

        public bool IsValid => !GetErrorMessages().Any();

        public IEnumerable<IStepDefinitionBinding> GetStepDefinitions()
        {
            return _stepDefinitions;
        }

        public IEnumerable<IStepDefinitionBinding> GetConsideredStepDefinitions(StepDefinitionType stepDefinitionType, string stepText)
        {
            //TODO: later optimize to return step definitions that has a chance to match to stepText
            return _stepDefinitions.Where(sd => sd.StepDefinitionType == stepDefinitionType);
        }

        public virtual IEnumerable<IHookBinding> GetHooks()
        {
            return _hooks.Values.SelectMany(hookList => hookList);
        }

        public virtual IEnumerable<IHookBinding> GetHooks(HookType bindingEvent)
        {
            return GetHookList(bindingEvent);
        }

        private IEnumerable<IHookBinding> GetHookList(HookType bindingEvent)
        {
            if (_hooks.TryGetValue(bindingEvent, out var list))
                return list;

            return Enumerable.Empty<IHookBinding>();
        }

        public virtual IEnumerable<IStepArgumentTransformationBinding> GetStepTransformations()
        {
            return _stepArgumentTransformations;
        }

        public IEnumerable<BindingError> GetErrorMessages()
        {
            foreach (var genericBindingError in _genericBindingErrors)
                yield return genericBindingError;

            foreach (var stepDefinitionBinding in _stepDefinitions.Where(sd => !sd.IsValid))
            {
                yield return new BindingError(BindingErrorType.StepDefinitionError, stepDefinitionBinding.ErrorMessage);
            }
        }

        public virtual void RegisterStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding)
        {
            _stepDefinitions.Add(stepDefinitionBinding);
        }

        private List<IHookBinding> GetHookListForRegister(HookType bindingEvent)
        {
            if (!_hooks.TryGetValue(bindingEvent, out var list))
            {
                list = new List<IHookBinding>();
                _hooks.Add(bindingEvent, list);
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
            _stepArgumentTransformations.Add(stepArgumentTransformationBinding);
        }

        public void RegisterGenericBindingError(BindingError error)
        {
            _genericBindingErrors.Add(error);
        }
    }
}