using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow
{
    public interface IValueRetriever
    {
        IEnumerable<Type> TypesForWhichIRetrieveValues();
        object ExtractValueFromRow(TableRow row, Type targetType);
    }
}