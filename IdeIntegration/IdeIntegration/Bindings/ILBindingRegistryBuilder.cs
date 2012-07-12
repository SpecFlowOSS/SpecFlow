using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.IdeIntegration.Bindings
{
    public class ILBindingRegistryBuilder
    {
        public void ProcessStepDefinitionsFromAssembly(string assemblyPath, IBindingSourceProcessor bindingSourceProcessor)
        {
            ModuleDefinition module = ModuleDefinition.ReadModule(assemblyPath);
            foreach (TypeDefinition typeDefinition in module.Types)
            {
// ReSharper disable PossibleMultipleEnumeration
                var filteredAttributes = typeDefinition.CustomAttributes.Where(attr => bindingSourceProcessor.CanProcessTypeAttribute(attr.AttributeType.FullName));
                if (!bindingSourceProcessor.PreFilterType(filteredAttributes.Select(attr => attr.AttributeType.FullName)))
                    continue;

                var bindingSourceType = CreateBindingSourceType(typeDefinition, filteredAttributes);

                if (!bindingSourceProcessor.ProcessType(bindingSourceType))
                    continue;

                foreach (var methodDefinition in typeDefinition.Methods)
                {
                    bindingSourceProcessor.ProcessMethod(CreateBindingSourceMethod(methodDefinition, bindingSourceType, bindingSourceProcessor));
                }

                bindingSourceProcessor.ProcessTypeDone();
// ReSharper restore PossibleMultipleEnumeration
            }
        }

        public IEnumerable<IStepDefinitionBinding> GetStepDefinitionsFromAssembly(string assemblyPath)
        {
            var bindingProcessor = new IdeBindingSourceProcessor();
            ProcessStepDefinitionsFromAssembly(assemblyPath, bindingProcessor);
            return bindingProcessor.ReadStepDefinitionBindings();
        }

        private BindingSourceMethod CreateBindingSourceMethod(MethodDefinition methodDefinition, BindingSourceType bindingSourceType, IBindingSourceProcessor bindingSourceProcessor)
        {
            return new BindingSourceMethod
                       {
                           BindingMethod = new BindingMethod(bindingSourceType.BindingType, methodDefinition.Name, GetParameters(methodDefinition), GetReturnType(methodDefinition)), 
                           IsPublic = methodDefinition.IsPublic,
                           IsStatic = methodDefinition.IsStatic,
                           Attributes = GetAttributes(methodDefinition.CustomAttributes.Where(attr => bindingSourceProcessor.CanProcessTypeAttribute(attr.AttributeType.FullName)))
                       };
        }

        private BindingType CreateBindingType(TypeReference typeReference)
        {
            return new BindingType(typeReference.Name, typeReference.FullName);
        }

        private IEnumerable<IBindingParameter> GetParameters(MethodDefinition methodDefinition)
        {
            return methodDefinition.Parameters.Select(pd => (IBindingParameter)new BindingParameter(CreateBindingType(pd.ParameterType), pd.Name));
        }

        private BindingType GetReturnType(MethodDefinition methodDefinition)
        {
            if (methodDefinition.ReturnType.FullName.Equals(typeof(void).FullName))
                return null;

            return CreateBindingType(methodDefinition.ReturnType);
        }

        private BindingSourceType CreateBindingSourceType(TypeDefinition typeDefinition, IEnumerable<CustomAttribute> filteredAttributes)
        {
            return new BindingSourceType
                       {
                           BindingType = CreateBindingType(typeDefinition),
                           IsAbstract = typeDefinition.IsAbstract,
                           IsClass = typeDefinition.IsClass,
                           IsPublic = typeDefinition.IsPublic,
                           IsNested = typeDefinition.IsNested,
                           IsGenericTypeDefinition = typeDefinition.HasGenericParameters,
                           Attributes = GetAttributes(filteredAttributes)
                       };
        }

        private BindingSourceAttribute CreateAttribute(CustomAttribute attribute)
        {
            return new BindingSourceAttribute
                       {
                           AttributeType = CreateBindingType(attribute.AttributeType), 
                           AttributeValues = attribute.ConstructorArguments.Select(CreateAttributeValue).ToArray(),
                           NamedAttributeValues = attribute.Fields.Concat(attribute.Properties).ToDictionary(na => na.Name, na => CreateAttributeValue(na.Argument))
                       };
        }

        private IBindingSourceAttributeValueProvider CreateAttributeValue(CustomAttributeArgument customAttributeArgument)
        {
            return new BindingSourceAttributeValueProvider(customAttributeArgument.Value);
        }

        private BindingSourceAttribute[] GetAttributes(IEnumerable<CustomAttribute> customAttributes)
        {
            return customAttributes.Select(CreateAttribute).ToArray();
        }
    }
}
