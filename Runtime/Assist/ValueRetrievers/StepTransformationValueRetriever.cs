using System;
using System.Linq;
using System.Globalization;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using System.Collections.Generic;
using BoDi;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class StepTransformationValueRetriever : IValueRetriever
    {
        private IObjectContainer container;

        public bool CanRetrieve(KeyValuePair<string, string> row, Type type)
        {
            try {
                return StepArgumentTypeConverter().CanConvert(row.Value, BindingTypeFor(type), CultureInfo());
            } catch {
                return false;
            }
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType)
        {
            return StepArgumentTypeConverter().Convert(keyValuePair.Value, BindingTypeFor(targetType), CultureInfo());
        }

        public IBindingType BindingTypeFor(Type type)
        {
            return new RuntimeBindingType(type);
        }

        public IStepArgumentTypeConverter StepArgumentTypeConverter()
        {
            return ObjectContainer().Resolve<IStepArgumentTypeConverter>();
        }

        public CultureInfo CultureInfo()
        {
            return ObjectContainer().Resolve<CultureInfo>();
        }

        public IObjectContainer Container {
            get;
            set;
        }

        public virtual IObjectContainer ObjectContainer()
        {
            if (Container != null)
                return Container;
            else
                return ScenarioContext.Current.ScenarioContainer;
        }
    }
}