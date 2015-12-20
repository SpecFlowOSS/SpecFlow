using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist
{
    public class Config
    {
        private List<IValueComparer> registeredValueComparers;
        private List<IValueRetriever> registeredValueRetrievers;

        static Config()
        {
            Instance = new Config();
        }

        public Config()
        {
            RestoreDefaults();
        }

        public IEnumerable<IValueComparer> ValueComparers
        {
            get { return registeredValueComparers; }
        }

        public IEnumerable<IValueRetriever> ValueRetrievers
        {
            get { return registeredValueRetrievers; }
        }

        public static Config Instance { get; internal set; }

        public void RestoreDefaults()
        {
            registeredValueComparers = new List<IValueComparer>();
            registeredValueRetrievers = new List<IValueRetriever>();
            RegisterSpecFlowDefaults();
        }

        public void RegisterValueComparer(IValueComparer valueComparer)
        {
            if (valueComparer.GetType() == typeof (DefaultValueComparer))
                registeredValueComparers.Add(valueComparer);
            else
                registeredValueComparers.Insert(0, valueComparer);
        }

        public void UnregisterValueComparer(IValueComparer valueComparer)
        {
            registeredValueComparers.Remove(valueComparer);
        }

        public void RegisterValueRetriever(IValueRetriever valueRetriever)
        {
            registeredValueRetrievers.Add(valueRetriever);
        }

        public void UnregisterValueRetriever(IValueRetriever valueRetriever)
        {
            registeredValueRetrievers.Remove(valueRetriever);
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
        }

        public IValueRetriever GetValueRetrieverFor(TableRow row, Type targetType, Type propertyType)
        {
            return ValueRetrievers.FirstOrDefault(r =>
            {
                var keyValuePair = new KeyValuePair<string, string>(row[0], row[1]);
                return r.CanRetrieve(keyValuePair, targetType, propertyType);
            });
        }
    }
}