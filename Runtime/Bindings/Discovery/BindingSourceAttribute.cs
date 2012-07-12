using System.Linq;
using System.Collections.Generic;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings.Discovery
{
    public class BindingSourceAttribute
    {
        public IBindingType AttributeType { get; set; }

        public IBindingSourceAttributeValueProvider[] AttributeValues { get; set; }
        public IDictionary<string, IBindingSourceAttributeValueProvider> NamedAttributeValues { get; set; }

        public TValue TryGetAttributeValue<TValue>(int index, TValue defaultValue = default(TValue))
        {
            if (AttributeValues.Length > index)
                return AttributeValues[index].GetValue<TValue>();
            return defaultValue;
        }

        public TValue[] TryGetParamsAttributeValue<TValue>(int index, TValue[] defaultValue = null)
        {
            if (AttributeValues.Length <= index)
                return defaultValue;

            return AttributeValues[index].GetValue<object>() as TValue[] ??
                   AttributeValues.Skip(index).OfType<TValue>().ToArray();
        }

        public TValue TryGetAttributeValue<TValue>(string name, TValue defaultValue = default(TValue))
        {
            IBindingSourceAttributeValueProvider valueProvider;
            if (NamedAttributeValues.TryGetValue(name, out valueProvider))
                return valueProvider.GetValue<TValue>();

            return defaultValue;
        }
    }
}