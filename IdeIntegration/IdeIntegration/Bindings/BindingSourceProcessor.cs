using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.IdeIntegration.Bindings
{
    public abstract class BindingSourceProcessor : IBindingSourceProcessor
    {
        public static string BINDING_ATTR = typeof(BindingAttribute).FullName;

        private readonly IBindingFactory bindingFactory;

        private BindingScope[] typeScopes;

        protected BindingSourceProcessor(IBindingFactory bindingFactory)
        {
            this.bindingFactory = bindingFactory;
        }

        public bool CanProcessTypeAttribute(string attributeTypeName)
        {
            return true;
        }

        public bool CanProcessMethodAttribute(string attributeTypeName)
        {
            return true;
        }

        public bool PreFilterType(IEnumerable<string> attributeTypeNames)
        {
            return attributeTypeNames.Any(attr => attr.Equals(BINDING_ATTR, StringComparison.InvariantCulture));
        }

        public bool ProcessType(BindingSourceType bindingSourceType)
        {
            typeScopes = null;

            if (!CheckType(bindingSourceType))
                return false;

            typeScopes = GetScopes(bindingSourceType.Attributes).ToArray();
            return true;
        }

        private IEnumerable<BindingScope> GetScopes(IEnumerable<BindingSourceAttribute> attributes)
        {
            return attributes.Where(attr => attr.AttributeType.TypeEquals(typeof(ScopeAttribute)))
                .Select(attr => new BindingScope(attr.TryGetAttributeValue<string>("Tag"), attr.TryGetAttributeValue<string>("Feature"), attr.TryGetAttributeValue<string>("Scenario")));
        }

        private bool CheckType(BindingSourceType bindingSourceType)
        {
            return bindingSourceType.Attributes.Any(attr => attr.AttributeType.TypeEquals(typeof(BindingAttribute))) &&
                   bindingSourceType.IsClass &&
                   !bindingSourceType.IsAbstract &&
                   !bindingSourceType.IsGenericTypeDefinition;
        }

        private bool IsStepDefinitionAttribute(BindingSourceAttribute attribute)
        {
            return
                attribute.AttributeType.TypeEquals(typeof(GivenAttribute)) ||
                attribute.AttributeType.TypeEquals(typeof(WhenAttribute)) ||
                attribute.AttributeType.TypeEquals(typeof(ThenAttribute)) ||
                attribute.AttributeType.TypeEquals(typeof(StepDefinitionAttribute));
        }

        public void ProcessMethod(BindingSourceMethod bindingSourceMethod)
        {
            var methodScopes = typeScopes.Concat(GetScopes(bindingSourceMethod.Attributes)).ToArray();

            ProcessStepDefinitions(bindingSourceMethod, methodScopes);
            ProcessHooks(bindingSourceMethod, methodScopes);
            ProcessStepArgumentTransformations(bindingSourceMethod, methodScopes);
        }

        protected virtual void ProcessStepArgumentTransformations(BindingSourceMethod bindingSourceMethod, BindingScope[] methodScopes)
        {
            //TODO
        }

        protected virtual void ProcessHooks(BindingSourceMethod bindingSourceMethod, BindingScope[] methodScopes)
        {
            //TODO
        }

        protected virtual void ProcessStepDefinitions(BindingSourceMethod bindingSourceMethod, BindingScope[] methodScopes)
        {
            foreach (var stepDefinitionAttribute in bindingSourceMethod.Attributes.Where(IsStepDefinitionAttribute))
            {
                ProcessStepDefinitionAttribute(bindingSourceMethod, methodScopes, stepDefinitionAttribute);
            }
        }

        private void ProcessStepDefinitionAttribute(BindingSourceMethod bindingSourceMethod, BindingScope[] methodScopes, BindingSourceAttribute stepDefinitionAttribute)
        {
            ApplyForScope(methodScopes, scope => ProcessStepDefinitionAttribute(bindingSourceMethod, stepDefinitionAttribute, scope));
        }

        private void ProcessStepDefinitionAttribute(BindingSourceMethod bindingSourceMethod, BindingSourceAttribute stepDefinitionAttribute, BindingScope scope)
        {
            var stepDefinitionTypes = GetStepDefinitionTypes(stepDefinitionAttribute);
            string regex = stepDefinitionAttribute.TryGetAttributeValue<string>(0);

            foreach (var stepDefinitionType in stepDefinitionTypes)
            {
                var stepDefinitionBinding = bindingFactory.CreateStepBinding(stepDefinitionType, regex, bindingSourceMethod.BindingMethod, scope);
                ProcessStepDefinitionBinding(stepDefinitionBinding);
            }
        }

        protected abstract void ProcessStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding);

        private IEnumerable<StepDefinitionType> GetStepDefinitionTypes(BindingSourceAttribute stepDefinitionAttribute)
        {
            if (stepDefinitionAttribute.AttributeType.TypeEquals(typeof(GivenAttribute)))
                return new[] { StepDefinitionType.Given };
            if (stepDefinitionAttribute.AttributeType.TypeEquals(typeof(WhenAttribute)))
                return new[] { StepDefinitionType.When };
            if (stepDefinitionAttribute.AttributeType.TypeEquals(typeof(ThenAttribute)))
                return new[] { StepDefinitionType.Then };
            if (stepDefinitionAttribute.AttributeType.TypeEquals(typeof(StepDefinitionAttribute)))
                return new[] { StepDefinitionType.Given, StepDefinitionType.When, StepDefinitionType.Then };

            return new StepDefinitionType[0];
        }

        private void ApplyForScope(BindingScope[] scopes, Action<BindingScope> action)
        {
            if (scopes.Any())
            {
                foreach (var scope in scopes)
                {
                    action(scope);
                }
            }
            else
            {
                action(null);
            }
        }
    }
}