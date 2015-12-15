using System;
using System.Collections;
using System.Collections.Generic;
using BoDi;
using System.Linq;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist
{
    public class Service
    {

        private List<IValueComparer> _registeredValueComparers;
        private List<IValueRetriever> _registeredValueRetrievers;

        public IEnumerable<IValueComparer> ValueComparers { get { return _registeredValueComparers; } }
        public IEnumerable<IValueRetriever> ValueRetrievers { get { return _registeredValueRetrievers; } }

        public static Service Instance { get; internal set; }

        static Service()
        {
            Instance = new Service();
        }

        public Service()
        {
            RestoreDefaults();
        }

        public void RestoreDefaults()
        {
            _registeredValueComparers = new List<IValueComparer>();
            _registeredValueRetrievers = new List<IValueRetriever>();
            RegisterSpecFlowDefaults();
        }

        public void RegisterValueComparer(IValueComparer valueComparer)
        {
            if (valueComparer.GetType() == typeof(DefaultValueComparer))
              _registeredValueComparers.Add(valueComparer);
            else
              _registeredValueComparers.Insert(0, valueComparer);
        }

        public void UnregisterValueComparer(IValueComparer valueComparer)
        {
            _registeredValueComparers.Remove(valueComparer);
        }

        public void RegisterValueRetriever(IValueRetriever valueRetriever)
        {
            _registeredValueRetrievers.Add(valueRetriever);
        }

        public void UnregisterValueRetriever(IValueRetriever valueRetriever)
        {
            _registeredValueRetrievers.Remove(valueRetriever);
        }

        public void RegisterSpecFlowDefaults()
        {
            RegisterValueComparer(new DateTimeValueComparer());
            RegisterValueComparer(new BoolValueComparer());
            RegisterValueComparer(new GuidValueComparer(new GuidValueRetriever()));
            RegisterValueComparer(new DecimalValueComparer());
            RegisterValueComparer(new DoubleValueComparer());
            RegisterValueComparer(new FloatValueComparer());
            RegisterValueComparer(new DefaultValueComparer());

            RegisterValueRetriever(new StringValueRetriever());
            RegisterValueRetriever(new ByteValueRetriever());
            RegisterValueRetriever(new SByteValueRetriever());
            RegisterValueRetriever(new IntValueRetriever());
            RegisterValueRetriever(new UIntValueRetriever());
            RegisterValueRetriever(new ShortValueRetriever());
            RegisterValueRetriever(new UShortValueRetriever());
            RegisterValueRetriever(new LongValueRetriever());
            RegisterValueRetriever(new ULongValueRetriever());
            RegisterValueRetriever(new FloatValueRetriever());
            RegisterValueRetriever(new DoubleValueRetriever());
            RegisterValueRetriever(new DecimalValueRetriever());
            RegisterValueRetriever(new CharValueRetriever());
            RegisterValueRetriever(new BoolValueRetriever());
            RegisterValueRetriever(new DateTimeValueRetriever());
            RegisterValueRetriever(new GuidValueRetriever());
            RegisterValueRetriever(new EnumValueRetriever());
            RegisterValueRetriever(new TimeSpanValueRetriever());
            RegisterValueRetriever(new DateTimeOffsetValueRetriever());
            RegisterValueRetriever(new NullableGuidValueRetriever());
            RegisterValueRetriever(new NullableDateTimeValueRetriever());
            RegisterValueRetriever(new NullableBoolValueRetriever());
            RegisterValueRetriever(new NullableCharValueRetriever());
            RegisterValueRetriever(new NullableDecimalValueRetriever());
            RegisterValueRetriever(new NullableDoubleValueRetriever());
            RegisterValueRetriever(new NullableFloatValueRetriever());
            RegisterValueRetriever(new NullableULongValueRetriever());
            RegisterValueRetriever(new NullableByteValueRetriever());
            RegisterValueRetriever(new NullableSByteValueRetriever());
            RegisterValueRetriever(new NullableIntValueRetriever());
            RegisterValueRetriever(new NullableUIntValueRetriever());
            RegisterValueRetriever(new NullableShortValueRetriever());
            RegisterValueRetriever(new NullableUShortValueRetriever());
            RegisterValueRetriever(new NullableLongValueRetriever());
            RegisterValueRetriever(new NullableTimeSpanValueRetriever());
            RegisterValueRetriever(new NullableDateTimeOffsetValueRetriever());
            RegisterValueRetriever(new StepTransformationValueRetriever());
        }

        public IValueRetriever GetValueRetrieverFor(TableRow row, Type targetType, Type propertyType)
        {
            foreach(var valueRetriever in ValueRetrievers){
                if (valueRetriever.CanRetrieve(new KeyValuePair<string, string>(row[0], row[1]), propertyType))
                    return valueRetriever;
            }
            return null;
        }

    }
}

