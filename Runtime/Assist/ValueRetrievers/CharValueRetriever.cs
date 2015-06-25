using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class CharValueRetriever : ValueRetrieverBase
    {
        public virtual char GetValue(string value)
        {
            return ThisStringIsNotASingleCharacter(value)
                       ? '\0'
                       : value[0];
        }
            
        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(char) };
        }

        private bool ThisStringIsNotASingleCharacter(string value)
        {
            return string.IsNullOrEmpty(value) || value.Length > 1;
        }
    }
}