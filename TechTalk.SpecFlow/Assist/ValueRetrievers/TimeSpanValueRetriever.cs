﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class TimeSpanValueRetriever : IValueRetriever
    {
        public virtual TimeSpan GetValue(string value)
        {
            return TimeSpan.Parse(value, CultureInfo.CurrentCulture);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(TimeSpan);
        }
    }
}