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

        public StepTransformationValueRetriever()
        {
            this.stepArgumentTypeConverter = null; // static container call here
            this.cultureInfo = null; // where can I get this?
        }

        public StepTransformationValueRetriever(IStepArgumentTypeConverter stepArgumentTypeConverter, CultureInfo cultureInfo)
        {
            this.stepArgumentTypeConverter = stepArgumentTypeConverter;
            this.cultureInfo = cultureInfo;
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            var bindingType = new RuntimeBindingType(targetType);
            return stepArgumentTypeConverter.Convert(row[1], bindingType, cultureInfo);
        }

        public bool CanRetrieve(Type type)
        {
            var bindingType = new RuntimeBindingType(type);
            return stepArgumentTypeConverter.CanConvert(null /* UGH */, bindingType, cultureInfo);
        }
    }
}

