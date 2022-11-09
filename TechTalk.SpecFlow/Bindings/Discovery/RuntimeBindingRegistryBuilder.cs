using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Compatibility;

namespace TechTalk.SpecFlow.Bindings.Discovery
{
    public class RuntimeBindingRegistryBuilder : IRuntimeBindingRegistryBuilder
    {
        private readonly IRuntimeBindingSourceProcessor _bindingSourceProcessor;
        private readonly ISpecFlowAttributesFilter _specFlowAttributesFilter;

        public RuntimeBindingRegistryBuilder(IRuntimeBindingSourceProcessor bindingSourceProcessor, ISpecFlowAttributesFilter specFlowAttributesFilter)
        {
            _bindingSourceProcessor = bindingSourceProcessor;
            _specFlowAttributesFilter = specFlowAttributesFilter;
        }

        public void BuildBindingsFromAssembly(Assembly assembly)
        {
            var assemblyTypes = GetAssemblyTypes(assembly, out var typeLoadErrors);
            foreach (var type in assemblyTypes)
            {
                BuildBindingsFromType(type);
            }
            if (typeLoadErrors != null)
                foreach (string typeLoadError in typeLoadErrors)
                    _bindingSourceProcessor.RegisterTypeLoadError(typeLoadError);
        }

        protected virtual Type[] GetAssemblyTypes(Assembly assembly, out string[] typeLoadErrors)
        {
            typeLoadErrors = Array.Empty<string>();
            //source: https://haacked.com/archive/2012/07/23/get-all-types-in-an-assembly.aspx/
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                typeLoadErrors = e.LoaderExceptions.Select(loaderException => loaderException.ToString()).ToArray();
                return e.Types.Where(t => t != null).ToArray();
            }
        }

        public void BuildingCompleted()
        {
            _bindingSourceProcessor.BuildingCompleted();
        }

        //internal - for testing
        internal bool BuildBindingsFromType(Type type)
        {
// ReSharper disable PossibleMultipleEnumeration
            var filteredAttributes = type.GetCustomAttributes(typeof(Attribute), true).Cast<Attribute>().Where(attr => _bindingSourceProcessor.CanProcessTypeAttribute(attr.GetType().FullName));
            if (!_bindingSourceProcessor.PreFilterType(filteredAttributes.Select(attr => attr.GetType().FullName)))
                return false;

            var bindingSourceType = CreateBindingSourceType(type, filteredAttributes);

            if (!_bindingSourceProcessor.ProcessType(bindingSourceType))
                return false;

            foreach (var methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                _bindingSourceProcessor.ProcessMethod(CreateBindingSourceMethod(methodInfo));
            }

            _bindingSourceProcessor.ProcessTypeDone();
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
                           Attributes = GetAttributes(methodDefinition.GetCustomAttributes(true).Cast<Attribute>().Where(attr => _bindingSourceProcessor.CanProcessTypeAttribute(attr.GetType().FullName)))
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

            var fieldsNamedAttributeValues =
                from field in attributeType.GetFields(BindingFlags.Instance | BindingFlags.Public)
                where !field.IsSpecialName
                let value = field.GetValue(attribute)
                let bindingSourceAttributeValueProvider = new BindingSourceAttributeValueProvider(value)
                select new { field.Name, Value = bindingSourceAttributeValueProvider };

            var propertiesNamedAttributeValues =
                from property in attributeType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                where !property.IsSpecialName
                where property.CanRead
                where property.GetIndexParameters().Length == 0
                let value = property.GetValue(attribute, null)
                let bindingSourceAttributeValueProvider = new BindingSourceAttributeValueProvider(value)
                select new { property.Name, Value = bindingSourceAttributeValueProvider };

            var namedAttributeValues =
                fieldsNamedAttributeValues.Concat(propertiesNamedAttributeValues)
                                          .ToDictionary(na => na.Name, na => (IBindingSourceAttributeValueProvider)na.Value);

            var mostComplexCtor = attributeType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).OrderByDescending(ctor => ctor.GetParameters().Length).FirstOrDefault();

            return new BindingSourceAttribute
            {
               AttributeType = CreateBindingType(attributeType),
               AttributeValues = mostComplexCtor == null ? Array.Empty<IBindingSourceAttributeValueProvider>() : mostComplexCtor.GetParameters().Select(p => FindAttributeConstructorArg(p, namedAttributeValues)).ToArray(),
               NamedAttributeValues = namedAttributeValues
            };
        }

        private IBindingSourceAttributeValueProvider FindAttributeConstructorArg(ParameterInfo parameterInfo, Dictionary<string, IBindingSourceAttributeValueProvider> namedAttributeValues)
        {
            var paramName = parameterInfo.Name;
            if (namedAttributeValues.TryGetValue(paramName, out var result))
                return result;
            if (namedAttributeValues.TryGetValue(paramName.Substring(0, 1).ToUpper() + paramName.Substring(1), out result))
                return result;

            return new BindingSourceAttributeValueProvider(null);
        }

        private BindingSourceAttribute[] GetAttributes(IEnumerable<Attribute> customAttributes)
        {
            return _specFlowAttributesFilter.FilterForSpecFlowAttributes(customAttributes)
                                            .Select(CreateAttribute)
                                            .ToArray();
        }
    }
}
