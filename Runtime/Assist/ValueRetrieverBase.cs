using System;
using System.Linq;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public abstract class ValueRetrieverBase : IValueRetriever
    {
        public abstract IEnumerable<Type> TypesForWhichIRetrieveValues();
        public abstract object ExtractValueFromRow(TableRow row, Type targetType);

        public virtual bool CanRetrieve(Type type)
        {
            return this.TypesForWhichIRetrieveValues().Contains(type);
        }
    }
}