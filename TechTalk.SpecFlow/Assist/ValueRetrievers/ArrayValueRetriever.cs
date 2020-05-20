using System;
using System.Collections;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ArrayValueRetriever : EnumerableValueRetriever
    {
        public override bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType.IsArray;
        }

        protected override Type GetActualValueType(Type propertyType)
        {
            return propertyType.GetElementType();
        }

        protected override object BuildInstance(int count, IEnumerable values, Type valueType)
        {
            var typedArray = Array.CreateInstance(valueType, count);
            int i = 0;
            foreach (var value in values)
            {
                typedArray.SetValue(value, i++);
            }
            return typedArray;
        }
    }
}