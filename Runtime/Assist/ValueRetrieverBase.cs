using System;
using System.Linq;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public abstract class ValueRetrieverBase : IValueRetriever
    {
        public abstract object ExtractValueFromRow(TableRow row, Type targetType);

        public virtual bool CanRetrieve(Type type)
        {
            return false;
        }
    }
}