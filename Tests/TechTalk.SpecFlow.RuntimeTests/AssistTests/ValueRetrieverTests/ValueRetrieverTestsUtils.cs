using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    internal static class ValueRetrieverTestsUtils
    {
        internal static bool CanRetrieve<TProperty>(this IValueRetriever retriever, string value)
        {
            return retriever.CanRetrieve(value, typeof(TProperty));
        }

        internal static bool CanRetrieve(this IValueRetriever retriever, string value, Type propertyType)
        {
            return retriever.CanRetrieve(new KeyValuePair<string, string>("Value", value), typeof(object), propertyType);
        }

        internal static TProperty GetValue<TProperty>(this IValueRetriever retriever, string value)
        {
            return (TProperty)retriever.GetValue(value, typeof(TProperty));
        }

        internal static object GetValue(this IValueRetriever retriever, string value, Type propertyType)
        {
            return retriever.Retrieve(new KeyValuePair<string, string>("Value", value), typeof(object), propertyType);
        }
    }
}