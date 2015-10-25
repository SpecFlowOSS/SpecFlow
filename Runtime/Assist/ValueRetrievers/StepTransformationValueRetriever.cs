using System;
using System.Linq;
using System.Globalization;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class StepTransformationValueRetriever : IValueRetriever
    {
        private IStepArgumentTypeConverter stepArgumentTypeConverter;
        private CultureInfo cultureInfo;

        public StepTransformationValueRetriever(IStepArgumentTypeConverter stepArgumentTypeConverter, CultureInfo cultureInfo)
        {
            this.stepArgumentTypeConverter = stepArgumentTypeConverter;
            this.cultureInfo = cultureInfo;
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return stepArgumentTypeConverter.Convert(row[1], BindingTypeFor(targetType), cultureInfo);
        }

        public bool CanRetrieve(TableRow row, Type type)
        {
            try {
                return stepArgumentTypeConverter.CanConvert(row[1], BindingTypeFor(type), cultureInfo);
            } catch {
                return false;
            }
        }

        public IBindingType BindingTypeFor(Type type)
        {
            return new RuntimeBindingType(type);
        }
    }
}