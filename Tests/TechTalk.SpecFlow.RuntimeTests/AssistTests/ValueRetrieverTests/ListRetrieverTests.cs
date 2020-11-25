using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class ListRetrieverTests : EnumerableRetrieverTests
    {
        protected override EnumerableValueRetriever CreateTestee()
        {
            return new ListValueRetriever();
        }

        protected override IEnumerable<Type> BuildPropertyTypes(Type valueType)
        {
            var propertyTypeDefinitions = new[]
            {
                typeof(IEnumerable<>),
                typeof(ICollection<>),
                typeof(IList<>),
                typeof(List<>),
                typeof(IReadOnlyList<>),
            };
            
            return propertyTypeDefinitions.Select(x => x.MakeGenericType(valueType));
        }
    }
}
