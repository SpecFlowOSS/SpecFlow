using System;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class VsBindingReflectionFactory
    {
        public BindingSourceType CreateBindingSourceType(CodeClass codeClass, CodeAttribute2[] filteredAttributes)
        {
            return new BindingSourceType
                       {
                           BindingType = CreateBindingType(codeClass),
                           IsAbstract = codeClass.IsAbstract,
                           IsClass = true,
                           IsPublic = codeClass.Access == vsCMAccess.vsCMAccessPublic,
                           IsNested = false, //TODO: codeClass.IsNested,
                           IsGenericTypeDefinition = (codeClass is CodeClass2) && ((CodeClass2)codeClass).IsGeneric,
                           Attributes = GetAttributes(filteredAttributes)
                       };
        }

        public BindingSourceMethod CreateBindingSourceMethod(CodeFunction codeFunction, BindingSourceType bindingSourceType, CodeAttribute2[] filteredAttributes)
        {
            return new BindingSourceMethod
                       {
                           BindingMethod = CreateBindingMethod(codeFunction), 
                           IsPublic = codeFunction.Access == vsCMAccess.vsCMAccessPublic,
                           IsStatic = (codeFunction.FunctionKind & vsCMFunction.vsCMFunctionShared) != 0,
                           Attributes = GetAttributes(filteredAttributes)
                       };
        }

        private BindingSourceAttribute[] GetAttributes(CodeAttribute2[] customAttributes)
        {
            return customAttributes.Select(CreateAttribute).ToArray();
        }

        private BindingSourceAttribute CreateAttribute(CodeAttribute2 attribute)
        {
            return new BindingSourceAttribute
                       {
                           AttributeType = CreateBindingType(attribute.FullName), 
                           AttributeValues = attribute.Arguments.Cast<CodeAttributeArgument>().Where(arg => string.IsNullOrEmpty(arg.Name)).Select(CreateAttributeValue).ToArray(),
                           NamedAttributeValues = attribute.Arguments.Cast<CodeAttributeArgument>().Where(arg => !string.IsNullOrEmpty(arg.Name)).ToDictionary(na => na.Name, CreateAttributeValue)
                       };
        }

        private class VsBindingSourceAttributeValueProvider: IBindingSourceAttributeValueProvider
        {
            private readonly CodeAttributeArgument customAttributeArgument;

            public VsBindingSourceAttributeValueProvider(CodeAttributeArgument customAttributeArgument)
            {
                this.customAttributeArgument = customAttributeArgument;
            }

            public TValue GetValue<TValue>()
            {
                if (typeof(TValue) == typeof(string))
                {
                    return (TValue)(object)VsxHelper.ParseCodeStringValue(customAttributeArgument.Value, customAttributeArgument.Language);
                }
                throw new NotSupportedException();
            }
        }

        private IBindingSourceAttributeValueProvider CreateAttributeValue(CodeAttributeArgument customAttributeArgument)
        {
            return new VsBindingSourceAttributeValueProvider(customAttributeArgument);
        }

        public IBindingType CreateBindingType(CodeTypeRef type)
        {
            if (type == null)
                return null;

            if (type.TypeKind == vsCMTypeRef.vsCMTypeRefVoid)
                return null;

            if (type.TypeKind == vsCMTypeRef.vsCMTypeRefArray)
            {
                var elementType = CreateBindingType(type.ElementType);
                int rank = type.Rank;
                return CreateBindingType(elementType.FullName + "[" + new string(',', rank - 1) + "]");
            }

            var fullName = type.AsFullName;
            return CreateBindingType(fullName);
        }

        private static IBindingType CreateBindingType(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Equals(typeof (void).FullName))
                return null;

            int lastPeriodIndex = fullName.LastIndexOf('.');
            var name = lastPeriodIndex >= 0 ? fullName.Substring(lastPeriodIndex + 1) : fullName;

            return new BindingType(name, fullName);
        }

        public IBindingType CreateBindingType(CodeClass codeClass)
        {
            return new BindingType(codeClass.Name, codeClass.FullName);
        }

        public IBindingParameter CreateBindingParameter(CodeParameter codeParameter)
        {
            return new BindingParameter(
                    CreateBindingType(codeParameter.Type), 
                    codeParameter.Name);
        }

        public IBindingMethod CreateBindingMethod(CodeFunction codeFunction)
        {
            var parameters = codeFunction.Parameters.Cast<CodeParameter>().Select(CreateBindingParameter).ToArray();
            var type = CreateBindingType((CodeClass)codeFunction.Parent);
            var returnType = CreateBindingType(codeFunction.Type);
            return new BindingMethod(type, codeFunction.Name, parameters, returnType);
        }
    }
}