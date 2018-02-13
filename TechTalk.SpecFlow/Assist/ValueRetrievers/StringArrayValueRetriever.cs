using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class StringArrayValueRetriever : IValueRetriever
    {
        const char ByCommaSeparator = ',';
        const char BySemiColonsSeparator = ';';


        public virtual string[] GetValue(string value)
        {

            var stringArray = value.Split(ByCommaSeparator, BySemiColonsSeparator).Select(p => p.Trim()).ToArray();

            return stringArray;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(string[]);
        }
    }
}