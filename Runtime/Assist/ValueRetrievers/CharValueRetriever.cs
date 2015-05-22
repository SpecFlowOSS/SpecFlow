using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class CharValueRetriever : IValueRetriever
    {
        public virtual char GetValue(string value)
        {
            return ThisStringIsNotASingleCharacter(value)
                       ? '\0'
                       : value[0];
        }
            
        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(char) };
        }

        private bool ThisStringIsNotASingleCharacter(string value)
        {
            return string.IsNullOrEmpty(value) || value.Length > 1;
        }
    }
}