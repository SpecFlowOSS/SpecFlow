using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using TechTalk.SpecFlow.Bindings.Discovery;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings.Discovery
{
    public class SpecFlowAttributesFilterTests
    {
        public static IEnumerable<object[]> GetAllSpecFlowAttributes()
        {
            var specFlowAssembly = typeof(BindingAttribute).Assembly;
            var parameterlessAttributes = GetAllParameterlessSpecFlowAttributes(specFlowAssembly);
            var attributesWithTags = GetAllSpecFlowAttributesWithRequiredTags(specFlowAssembly);
            var attributes = parameterlessAttributes.Concat(attributesWithTags);

            return from attribute in attributes
                   select new object[] { attribute };
        }

        private static IEnumerable<Attribute> GetAllParameterlessSpecFlowAttributes(Assembly specFlowAssembly)
        {
            var attribute = from type in specFlowAssembly.GetTypes()
                            where type.IsSubclassOf(typeof(Attribute))
                            where !type.IsAbstract
                            where type.IsPublic
                            where type.IsClass
                            where type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Any(c => c.GetParameters().Length == 0)
                            select (Attribute)Activator.CreateInstance(type);
            return attribute;
        }

        private static IEnumerable<Attribute> GetAllSpecFlowAttributesWithRequiredTags(Assembly specFlowAssembly)
        {
            var attribute = from type in specFlowAssembly.GetTypes()
                            where type.IsSubclassOf(typeof(Attribute))
                            where !type.IsAbstract
                            where type.IsPublic
                            where type.IsClass
                            where type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Any(c => c.GetParameters().Length > 0 && c.GetParameters()[0].ParameterType == typeof(string[]))
                            select (Attribute)Activator.CreateInstance(type, new object[] { new string[0] });
            return attribute;
        }

        [Theory]
        [MemberData(nameof(GetAllSpecFlowAttributes))]
        public void FilterForSpecFlowAttributes_SpecFlowAttributes_ShouldReturnAllSpecFlowAttributes(Attribute attribute)
        {
            // ARRANGE
            var specFlowAttributesFilter = new SpecFlowAttributesFilter();
            var attributesToFilter = new[] { attribute };

            // ACT
            var filteredAttributes = specFlowAttributesFilter.FilterForSpecFlowAttributes(attributesToFilter);

            // ASSERT
            filteredAttributes.Should().BeEquivalentTo(attributesToFilter.AsEnumerable());
        }
    }
}
