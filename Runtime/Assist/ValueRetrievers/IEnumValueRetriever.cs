using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public interface IEnumValueRetriever : IValueRetriever
    {
        object GetValue(string text, Type enumType);
    }
}