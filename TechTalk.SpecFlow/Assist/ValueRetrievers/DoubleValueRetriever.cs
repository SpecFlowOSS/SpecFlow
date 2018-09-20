﻿using System;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DoubleValueRetriever : NonNullableValueRetriever<double>
    {
        public override double GetValue(string value)
        {
            Double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out double returnValue);
            return returnValue;
        }
    }
}