﻿using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ArrayValueRetriever : EnumerableValueRetriever
    {
        protected override Type GetActualValueType(Type propertyType)
        {
            return propertyType.IsArray ? propertyType.GetElementType() : null;
        }

        protected override object BuildInstance(object[] values, Type valueType)
        {
            var typedArray = Array.CreateInstance(valueType, values.Length);
            Array.Copy(values, typedArray, values.Length);
            return typedArray;
        }
    }
}