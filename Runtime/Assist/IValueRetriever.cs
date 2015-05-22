using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public interface IValueRetriever
    {
        IEnumerable<Type> TypesForWhichIRetrieveValues();
        object ExtractValueFromRow(TableRow row, Type targetType);
    }
}