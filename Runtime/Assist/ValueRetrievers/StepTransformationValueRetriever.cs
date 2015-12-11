using System;
using System.Linq;
using System.Globalization;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using System.Collections.Generic;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class StepTransformationValueRetriever : IValueRetriever
    {
        private readonly IContextManager contextManager;
        private readonly IStepArgumentTypeConverter stepArgumentTypeConverter;

        public StepTransformationValueRetriever(IContextManager contextManager, IStepArgumentTypeConverter stepArgumentTypeConverter)
        {
            this.contextManager = contextManager;
            this.stepArgumentTypeConverter = stepArgumentTypeConverter;
        }

        public bool CanRetrieve(KeyValuePair<string, string> row, Type type)
        {
            try {
                return stepArgumentTypeConverter.CanConvert(row.Value, BindingTypeFor(type), GetBindingCulture());
            } catch {
                return false;
            }
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType)
        {
            return stepArgumentTypeConverter.Convert(keyValuePair.Value, BindingTypeFor(targetType), GetBindingCulture());
        }

        private IBindingType BindingTypeFor(Type type)
        {
            return new RuntimeBindingType(type);
        }

        private CultureInfo GetBindingCulture()
        {
            return contextManager.FeatureContext.BindingCulture;
        }
    }
}