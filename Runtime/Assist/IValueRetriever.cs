using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public interface IValueRetriever
    {
        object ExtractValueFromRow(TableRow row, Type targetType);
        bool CanRetrieve(Type type);
    }
}