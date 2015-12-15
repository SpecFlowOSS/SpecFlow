using System;
using System.Linq;
using System.Globalization;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using System.Collections.Generic;
using BoDi;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class StepTransformationValueRetriever : IValueRetriever
    {
        public bool CanRetrieve(KeyValuePair<string, string> row, Type type)
        {
            try {
                return StepArgumentTypeConverter().CanConvert(row.Value, BindingTypeFor(type), CultureInfo());
            } catch {
                return false;
            }
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return StepArgumentTypeConverter().Convert(keyValuePair.Value, BindingTypeFor(propertyType), CultureInfo());
        }

        public IBindingType BindingTypeFor(Type type)
        {
            return new RuntimeBindingType(type);
        }

        public IStepArgumentTypeConverter StepArgumentTypeConverter()
        {
            return Container().Resolve<IStepArgumentTypeConverter>();
        }

        public CultureInfo CultureInfo()
        {
            var contextManager = Container().Resolve<IContextManager>();
            return contextManager.FeatureContext.BindingCulture;
        }

        public IObjectContainer ContainerToUseForThePurposeOfTesting { get; set; }

        public virtual IObjectContainer Container()
        {
            if (ContainerToUseForThePurposeOfTesting != null)
                return ContainerToUseForThePurposeOfTesting;
            else
                return ScenarioContext.Current.ScenarioContainer;
        }
    }
}