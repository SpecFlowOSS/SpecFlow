using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public interface IValueRetriever
    {
        object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType);
        bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type type);
    }
}