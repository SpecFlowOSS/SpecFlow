using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class ArrayRetrieverTests : EnumerableRetrieverTests
    {
        protected override EnumerableValueRetriever CreateTestee()
        {
            return new ArrayValueRetriever();
        }

        protected override IEnumerable<Type> BuildPropertyTypes(Type valueType)
        {
            yield return valueType.MakeArrayType();
        }
    }
}