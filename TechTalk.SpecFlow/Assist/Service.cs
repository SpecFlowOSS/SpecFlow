using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist
{
    public class Service
    {

        private List<IValueComparer> _registeredValueComparers;
        private List<IValueRetriever> _registeredValueRetrievers;

        public IEnumerable<IValueComparer> ValueComparers => _registeredValueComparers;
        public IEnumerable<IValueRetriever> ValueRetrievers => _registeredValueRetrievers;

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
            _registeredValueComparers.Insert(0, valueComparer);
        }

        public void RegisterValueComparer<TValueComparer>() where TValueComparer : IValueComparer
        {
            var valueComparer = Activator.CreateInstance<TValueComparer>();

            RegisterValueComparer(valueComparer);
        }

        public void RegisterDefaultValueComparer(IValueComparer valueComparer)
        {
            _registeredValueComparers.Add(valueComparer);
        }

        public void UnregisterValueComparer(IValueComparer valueComparer)
        {
            _registeredValueComparers.Remove(valueComparer);
        }

        public void UnregisterValueComparer<TValueComparer>() where TValueComparer : IValueComparer
        {
            var valueComparer = _registeredValueComparers.FirstOrDefault(x => x.GetType() == typeof(TValueComparer));

            UnregisterValueComparer(valueComparer);
        }

        public void RegisterValueRetriever(IValueRetriever valueRetriever)
        {
            _registeredValueRetrievers.Add(valueRetriever);
        }

        public void RegisterValueRetriever<TValueRetriever>() where TValueRetriever : IValueRetriever
        {
            var valueRetriever = Activator.CreateInstance<TValueRetriever>();

            RegisterValueRetriever(valueRetriever);
        }

        public void UnregisterValueRetriever(IValueRetriever valueRetriever)
        {
            _registeredValueRetrievers.Remove(valueRetriever);
        }

        public void UnregisterValueRetriever<TValueRetriever>() where TValueRetriever : IValueRetriever
        {
            var valueRetriver = _registeredValueRetrievers.FirstOrDefault(x => x.GetType() == typeof(TValueRetriever));
            
            UnregisterValueRetriever(valueRetriver);
        }

        public void RegisterSpecFlowDefaults()
        {
            RegisterValueComparer<DateTimeValueComparer>();
            RegisterValueComparer<BoolValueComparer>();
            RegisterValueComparer(new GuidValueComparer(new GuidValueRetriever()));
            RegisterValueComparer<DecimalValueComparer>();
            RegisterValueComparer<DoubleValueComparer>();
            RegisterValueComparer<FloatValueComparer>();
            RegisterDefaultValueComparer(new DefaultValueComparer());

            RegisterValueRetriever<StringValueRetriever>();
            RegisterValueRetriever<ByteValueRetriever>();
            RegisterValueRetriever<SByteValueRetriever>();
            RegisterValueRetriever<IntValueRetriever>();
            RegisterValueRetriever<UIntValueRetriever>();
            RegisterValueRetriever<ShortValueRetriever>();
            RegisterValueRetriever<UShortValueRetriever>();
            RegisterValueRetriever<LongValueRetriever>();
            RegisterValueRetriever<ULongValueRetriever>();
            RegisterValueRetriever<FloatValueRetriever>();
            RegisterValueRetriever<DoubleValueRetriever>();
            RegisterValueRetriever<DecimalValueRetriever>();
            RegisterValueRetriever<CharValueRetriever>();
            RegisterValueRetriever<BoolValueRetriever>();
            RegisterValueRetriever<DateTimeValueRetriever>();
            RegisterValueRetriever<GuidValueRetriever>();
            RegisterValueRetriever<EnumValueRetriever>();
            RegisterValueRetriever<TimeSpanValueRetriever>();
            RegisterValueRetriever<DateTimeOffsetValueRetriever>();
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

            RegisterValueRetriever<StringArrayValueRetriever>();
            RegisterValueRetriever<StringListValueRetriever>();
            RegisterValueRetriever<EnumArrayValueRetriever>();
            RegisterValueRetriever<EnumListValueRetriever>();
        }

        public IValueRetriever GetValueRetrieverFor(TableRow row, Type targetType, Type propertyType)
        {
            foreach(var valueRetriever in ValueRetrievers){
                if (valueRetriever.CanRetrieve(new KeyValuePair<string, string>(row[0], row[1]), targetType, propertyType))
                    return valueRetriever;
            }
            return null;
        }

    }
}

