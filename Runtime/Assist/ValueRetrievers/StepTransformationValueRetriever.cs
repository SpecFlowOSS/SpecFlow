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

        public virtual object GetValue(string value, Type type)
        {
            IBindingType bindingType = null; // gotta get this from type?
            return stepArgumentTypeConverter.Convert(value, bindingType, cultureInfo);
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1], null /* UGH */);
        }

        public IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            // UH OH!!!!!!!!!!
            return new Type[]{};
        }
    }
}

