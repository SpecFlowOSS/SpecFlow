using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Compatibility;

namespace TechTalk.SpecFlow.Bindings.Discovery
{
    public abstract class BindingSourceProcessor : IBindingSourceProcessor
    {
        public static string BINDING_ATTR = typeof(BindingAttribute).FullName;

        private readonly IBindingFactory bindingFactory;

        private BindingSourceType currentBindingSourceType = null;
        private BindingScope[] typeScopes = null;

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

        private static bool IsPotentialBindingClass(IEnumerable<string> attributeTypeNames)
        {
            return attributeTypeNames.Any(attr => attr.Equals(BINDING_ATTR, StringComparison.InvariantCulture));
        }

        public bool PreFilterType(IEnumerable<string> attributeTypeNames)
        {
            return IsPotentialBindingClass(attributeTypeNames);
        }

        public bool ProcessType(BindingSourceType bindingSourceType)
        {
            typeScopes = null;

            if (!IsBindingType(bindingSourceType))
                return false;

            if (!ValidateType(bindingSourceType))
                return false;

            currentBindingSourceType = bindingSourceType;
            typeScopes = GetScopes(bindingSourceType.Attributes).ToArray();
            return true;
        }

        public void ProcessTypeDone()
        {
            currentBindingSourceType = null;
            typeScopes = null;
        }

        private IEnumerable<BindingScope> GetScopes(IEnumerable<BindingSourceAttribute> attributes)
        {
            return attributes.Where(attr => attr.AttributeType.TypeEquals(typeof(ScopeAttribute)))
                .Select(attr => new BindingScope(attr.TryGetAttributeValue<string>("Tag"), attr.TryGetAttributeValue<string>("Feature"), attr.TryGetAttributeValue<string>("Scenario")));
        }

        private bool IsBindingType(BindingSourceType bindingSourceType)
        {
            return bindingSourceType.Attributes.Any(attr => attr.AttributeType.TypeEquals(typeof(BindingAttribute)));
        }

        private bool IsStepDefinitionAttribute(BindingSourceAttribute attribute)
        {
            return
                attribute.AttributeType.TypeEquals(typeof(GivenAttribute)) ||
                attribute.AttributeType.TypeEquals(typeof(WhenAttribute)) ||
                attribute.AttributeType.TypeEquals(typeof(ThenAttribute)) ||
                attribute.AttributeType.TypeEquals(typeof(StepDefinitionAttribute));
        }

        private bool IsHookAttribute(BindingSourceAttribute attribute)
        {
// ReSharper disable AssignNullToNotNullAttribute
            return attribute.AttributeType.FullName.StartsWith(typeof(BeforeScenarioAttribute).Namespace) &&
                TryGetHookType(attribute) != null;
// ReSharper restore AssignNullToNotNullAttribute
        }

        private bool IsStepArgumentTransformationAttribute(BindingSourceAttribute attribute)
        {
            return attribute.AttributeType.TypeEquals(typeof(StepArgumentTransformationAttribute));
        }

        public void ProcessMethod(BindingSourceMethod bindingSourceMethod)
        {
            var methodScopes = typeScopes.Concat(GetScopes(bindingSourceMethod.Attributes)).ToArray();

            ProcessStepDefinitions(bindingSourceMethod, methodScopes);
            ProcessHooks(bindingSourceMethod, methodScopes);
            ProcessStepArgumentTransformations(bindingSourceMethod, methodScopes);
        }

        protected virtual void ProcessStepDefinitions(BindingSourceMethod bindingSourceMethod, BindingScope[] methodScopes)
        {
            foreach (var stepDefinitionAttribute in bindingSourceMethod.Attributes.Where(IsStepDefinitionAttribute))
            {
                ProcessStepDefinitionAttribute(bindingSourceMethod, methodScopes, stepDefinitionAttribute);
            }
        }

        protected virtual void ProcessHooks(BindingSourceMethod bindingSourceMethod, BindingScope[] methodScopes)
        {
            foreach (var hookAttribute in bindingSourceMethod.Attributes.Where(IsHookAttribute))
            {
                ProcessHookAttribute(bindingSourceMethod, methodScopes, hookAttribute);
            }
        }

        protected virtual void ProcessStepArgumentTransformations(BindingSourceMethod bindingSourceMethod, BindingScope[] methodScopes)
        {
            foreach (var stepArgumentTransformationAttribute in bindingSourceMethod.Attributes.Where(IsStepArgumentTransformationAttribute))
            {
                ProcessStepArgumentTransformationAttribute(bindingSourceMethod, stepArgumentTransformationAttribute);
            }
        }

        private void ProcessStepDefinitionAttribute(BindingSourceMethod bindingSourceMethod, BindingScope[] methodScopes, BindingSourceAttribute stepDefinitionAttribute)
        {
            ApplyForScope(methodScopes, scope => ProcessStepDefinitionAttribute(bindingSourceMethod, stepDefinitionAttribute, scope));
        }

        private void ProcessHookAttribute(BindingSourceMethod bindingSourceMethod, BindingScope[] methodScopes, BindingSourceAttribute hookAttribute)
        {
            var scopes = methodScopes.AsEnumerable();

			// HACK: Currently on mono to compile we have to pass the optional parameter to TryGetParamsAttributeValue
            string[] tags = hookAttribute.TryGetParamsAttributeValue<string>(0, null);
            if (tags != null)
                scopes = scopes.Concat(tags.Select(t => new BindingScope(t, null, null)));

            ApplyForScope(scopes.ToArray(), scope => ProcessHookAttribute(bindingSourceMethod, hookAttribute, scope));
        }

        private void ProcessHookAttribute(BindingSourceMethod bindingSourceMethod, BindingSourceAttribute hookAttribute, BindingScope scope)
        {
            HookType hookType = GetHookType(hookAttribute);

            if (!ValidateHook(bindingSourceMethod, hookAttribute, hookType))
                return;

            var hookBinding = bindingFactory.CreateHookBinding(bindingSourceMethod.BindingMethod, hookType, scope);

            ProcessHookBinding(hookBinding);
        }

        private void ProcessStepArgumentTransformationAttribute(BindingSourceMethod bindingSourceMethod, BindingSourceAttribute stepArgumentTransformationAttribute)
        {
            string regex = stepArgumentTransformationAttribute.TryGetAttributeValue<string>(0) ?? stepArgumentTransformationAttribute.TryGetAttributeValue<string>("Regex");

            if (!ValidateStepArgumentTransformation(bindingSourceMethod, stepArgumentTransformationAttribute))
                return;

            var stepArgumentTransformationBinding = bindingFactory.CreateStepArgumentTransformation(regex, bindingSourceMethod.BindingMethod);

            ProcessStepArgumentTransformationBinding(stepArgumentTransformationBinding);
        }

        private HookType GetHookType(BindingSourceAttribute hookAttribute)
        {
            var hookType = TryGetHookType(hookAttribute);
            if (hookType == null)
                throw new SpecFlowException("Invalid hook attribute: " + hookAttribute);
            return hookType.Value;
        }

        private static readonly string[] hookNames = EnumHelper.GetNames(typeof(HookType));
        private HookType? TryGetHookType(BindingSourceAttribute hookAttribute)
        {
            string typeName = hookAttribute.AttributeType.Name;

            if (typeName == typeof(BeforeAttribute).Name)
                return HookType.BeforeScenario;
            if (typeName == typeof(AfterAttribute).Name)
                return HookType.AfterScenario;

            const string attributePostfix = "Attribute";
            if (!typeName.EndsWith(attributePostfix))
                return null;
            string name = typeName.Substring(0, typeName.Length - attributePostfix.Length);

            if (!hookNames.Contains(name))
                return null;

            return (HookType)Enum.Parse(typeof(HookType), name, false);
        }

        private void ProcessStepDefinitionAttribute(BindingSourceMethod bindingSourceMethod, BindingSourceAttribute stepDefinitionAttribute, BindingScope scope)
        {
            var stepDefinitionTypes = GetStepDefinitionTypes(stepDefinitionAttribute);
            string regex = stepDefinitionAttribute.TryGetAttributeValue<string>(0);

            if (!ValidateStepDefinition(bindingSourceMethod, stepDefinitionAttribute))
                return;

            foreach (var stepDefinitionType in stepDefinitionTypes)
            {
                var stepDefinitionBinding = bindingFactory.CreateStepBinding(stepDefinitionType, regex, bindingSourceMethod.BindingMethod, scope);
                ProcessStepDefinitionBinding(stepDefinitionBinding);
            }
        }

        protected virtual bool OnValidationError(string messageFormat, params object[] arguments)
        {
            return false;
        }

        protected virtual bool ValidateType(BindingSourceType bindingSourceType)
        {
            if (!bindingSourceType.IsClass && 
                OnValidationError("Binding types must be classes: {0}", bindingSourceType))
                    return false;

            if (bindingSourceType.IsGenericTypeDefinition && 
                OnValidationError("Binding types cannot be generic: {0}", bindingSourceType))
                    return false;

            return true;
        }

        protected virtual bool ValidateMethod(BindingSourceMethod bindingSourceMethod)
        {
            if (currentBindingSourceType.IsAbstract && !bindingSourceMethod.IsStatic && 
                OnValidationError("Abstract binding types can have only static binding methods: {0}", bindingSourceMethod))
                    return false;

            return true;
        }

        protected virtual bool ValidateStepDefinition(BindingSourceMethod bindingSourceMethod, BindingSourceAttribute stepDefinitionAttribute)
        {
            if (!ValidateMethod(bindingSourceMethod))
                return false;

            return true;
        }

        protected virtual bool ValidateHook(BindingSourceMethod bindingSourceMethod, BindingSourceAttribute hookAttribute, HookType hookType)
        {
            if (!ValidateMethod(bindingSourceMethod))
                return false;

            if (!IsScenarioSpecificHook(hookType) && !bindingSourceMethod.IsStatic &&
                OnValidationError("The binding methods for before/after feature and before/after test run events must be static! {0}", bindingSourceMethod))
                return false;

            return true;
        }

        protected bool IsScenarioSpecificHook(HookType hookType)
        {
            return
                hookType == HookType.BeforeScenario ||
                hookType == HookType.AfterScenario ||
                hookType == HookType.BeforeScenarioBlock ||
                hookType == HookType.AfterScenarioBlock ||
                hookType == HookType.BeforeStep ||
                hookType == HookType.AfterStep;
        }

        protected virtual bool ValidateStepArgumentTransformation(BindingSourceMethod bindingSourceMethod, BindingSourceAttribute stepArgumentTransformationAttribute)
        {
            if (!ValidateMethod(bindingSourceMethod))
                return false;

            return true;
        }

        protected abstract void ProcessStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding);
        protected abstract void ProcessHookBinding(IHookBinding hookBinding);
        protected abstract void ProcessStepArgumentTransformationBinding(IStepArgumentTransformationBinding stepArgumentTransformationBinding);

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