using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Compatibility;

namespace TechTalk.SpecFlow.Bindings.Discovery
{
    public class RuntimeBindingRegistryBuilder : IRuntimeBindingRegistryBuilder
    {
        private readonly IRuntimeBindingSourceProcessor bindingSourceProcessor;

        public RuntimeBindingRegistryBuilder(IRuntimeBindingSourceProcessor bindingSourceProcessor)
        {
            this.bindingSourceProcessor = bindingSourceProcessor;
        }

        public void BuildBindingsFromAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                BuildBindingsFromType(type);
            }
        }

        //internal - for testing
        internal bool BuildBindingsFromType(Type type)
        {
// ReSharper disable PossibleMultipleEnumeration
            var filteredAttributes = type.GetCustomAttributes(typeof(Attribute), true).Cast<Attribute>().Where(attr => bindingSourceProcessor.CanProcessTypeAttribute(attr.GetType().FullName));
            if (!bindingSourceProcessor.PreFilterType(filteredAttributes.Select(attr => attr.GetType().FullName)))
                return false;

            var bindingSourceType = CreateBindingSourceType(type, filteredAttributes);

            if (!bindingSourceProcessor.ProcessType(bindingSourceType))
                return false;

            foreach (var methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                bindingSourceProcessor.ProcessMethod(CreateBindingSourceMethod(methodInfo));
            }

            bindingSourceProcessor.ProcessTypeDone();
            return true;
// ReSharper restore PossibleMultipleEnumeration
        }

        private BindingSourceMethod CreateBindingSourceMethod(MethodInfo methodDefinition)
        {
            return new BindingSourceMethod
                       {
                           BindingMethod = new RuntimeBindingMethod(methodDefinition), 
                           IsPublic = methodDefinition.IsPublic,
                           IsStatic = methodDefinition.IsStatic,
                           Attributes = GetAttributes(methodDefinition.GetCustomAttributes(true).Cast<Attribute>().Where(attr => bindingSourceProcessor.CanProcessTypeAttribute(attr.GetType().FullName)))
                       };
        }

        private IBindingType CreateBindingType(Type type)
        {
            return new RuntimeBindingType(type);
        }

        private BindingSourceType CreateBindingSourceType(Type type, IEnumerable<Attribute> filteredAttributes)
        {
            return new BindingSourceType
                       {
                           BindingType = CreateBindingType(type),
                           IsAbstract = type.IsAbstract,
                           IsClass = type.IsClass,
                           IsPublic = type.IsPublic,
                           IsNested = TypeHelper.IsNested(type),
                           IsGenericTypeDefinition = type.IsGenericTypeDefinition,
                           Attributes = GetAttributes(filteredAttributes)
                       };
        }

        private BindingSourceAttribute CreateAttribute(Attribute attribute)
        {
            var attributeType = attribute.GetType();
            var namedAttributeValues = attributeType.GetFields(BindingFlags.Instance | BindingFlags.Public).Where(
                f => !f.IsSpecialName).Select(
                    f => new {f.Name, Value = new BindingSourceAttributeValueProvider(f.GetValue(attribute))})
                .Concat(
                    attributeType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(
                        p => !p.IsSpecialName && p.CanRead && p.GetIndexParameters().Length == 0).Select(
                            p =>
                            new {p.Name, Value = new BindingSourceAttributeValueProvider(p.GetValue(attribute, null))})
                ).ToDictionary(na => na.Name, na => (IBindingSourceAttributeValueProvider)na.Value);

            var mostComplexCtor = attributeType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).OrderByDescending(ctor => ctor.GetParameters().Length).FirstOrDefault();

            return new BindingSourceAttribute
                       {
                           AttributeType = CreateBindingType(attributeType), 
                           AttributeValues = mostComplexCtor == null ? new IBindingSourceAttributeValueProvider[0] : mostComplexCtor.GetParameters().Select(p => FindAttributeConstructorArg(p, namedAttributeValues)).ToArray(),
                           NamedAttributeValues = namedAttributeValues
                       };
        }

        private IBindingSourceAttributeValueProvider FindAttributeConstructorArg(ParameterInfo parameterInfo, Dictionary<string, IBindingSourceAttributeValueProvider> namedAttributeValues)
        {
            IBindingSourceAttributeValueProvider result;
            var paramName = parameterInfo.Name;
            if (namedAttributeValues.TryGetValue(paramName, out result))
                return result;
            if (namedAttributeValues.TryGetValue(paramName.Substring(0, 1).ToUpper() + paramName.Substring(1), out result))
                return result;

            return new BindingSourceAttributeValueProvider(null);
        }

        private BindingSourceAttribute[] GetAttributes(IEnumerable<Attribute> customAttributes)
        {
            return customAttributes.Select(CreateAttribute).ToArray();
        }
    }
}
