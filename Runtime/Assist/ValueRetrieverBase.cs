using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public abstract class ValueRetrieverBase : IValueRetriever
    {
        public abstract IEnumerable<Type> TypesForWhichIRetrieveValues();
        public abstract object ExtractValueFromRow(TableRow row, Type targetType);
    }
}