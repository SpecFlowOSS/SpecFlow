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

        private Dictionary<string, IValueComparer> _valueComparers = new Dictionary<string, IValueComparer>();
        private Dictionary<string, IValueRetriever> _theValueRetrievers = new Dictionary<string, IValueRetriever>();
        private IDictionary<Type, Func<TableRow, Type, object>> _valueRetrievers = new Dictionary<Type, Func<TableRow, Type, object>>();

        public static Service Instance { get; internal set; }

        public IDictionary<string, IValueComparer> ValueComparers { get { return _valueComparers; } }
        public IDictionary<Type, Func<TableRow, Type, object>> ValueRetrievers { get { return _valueRetrievers; } }
        public IDictionary<string, IValueRetriever> TheValueRetrievers { get { return _theValueRetrievers; } }

        static Service()
        {
            Instance = new Service();
        }

        public Service()
        {
            RegisterValueComparer(new DateTimeValueComparer(), "datetime");
            RegisterValueComparer(new BoolValueComparer(), "bool");
            RegisterValueComparer(new GuidValueComparer(new GuidValueRetriever()), "guid");
            RegisterValueComparer(new DecimalValueComparer(), "decimal");
            RegisterValueComparer(new DoubleValueComparer(), "double");
            RegisterValueComparer(new FloatValueComparer(), "float");
            RegisterValueComparer(new DefaultValueComparer(), "default");

            var someValueRetrievers = new IValueRetriever[]{
                new StringValueRetriever(),
                new ByteValueRetriever(),
                new SByteValueRetriever(),
                new IntValueRetriever(),
                new UIntValueRetriever(),
                new ShortValueRetriever(),
                new UShortValueRetriever(),
                new LongValueRetriever(),
                new ULongValueRetriever(),
                new FloatValueRetriever(),
                new DoubleValueRetriever(),
                new DecimalValueRetriever(),
                new CharValueRetriever(),
                new BoolValueRetriever(),
                new DateTimeValueRetriever(),
                new GuidValueRetriever(),
                new EnumValueRetriever(),
                new NullableGuidValueRetriever(v => new GuidValueRetriever().GetValue(v)),
                new NullableDateTimeValueRetriever(v => new DateTimeValueRetriever().GetValue(v)),
                new NullableBoolValueRetriever(v => new BoolValueRetriever().GetValue(v)),
                new NullableCharValueRetriever(v => new CharValueRetriever().GetValue(v)),
                new NullableDecimalValueRetriever(v => new DecimalValueRetriever().GetValue(v)),
                new NullableDoubleValueRetriever(v => new DoubleValueRetriever().GetValue(v)),
                new NullableFloatValueRetriever(v => new FloatValueRetriever().GetValue(v)),
                new NullableULongValueRetriever(v => new ULongValueRetriever().GetValue(v)),
                new NullableByteValueRetriever(v => new ByteValueRetriever().GetValue(v)),
                new NullableSByteValueRetriever(v => new SByteValueRetriever().GetValue(v)),
                new NullableIntValueRetriever(v => new IntValueRetriever().GetValue(v)),
                new NullableUIntValueRetriever(v => new UIntValueRetriever().GetValue(v)),
                new NullableShortValueRetriever(v => new ShortValueRetriever().GetValue(v)),
                new NullableUShortValueRetriever(v => new UShortValueRetriever().GetValue(v)),
                new NullableLongValueRetriever(v => new LongValueRetriever().GetValue(v)),
            };

            foreach(var valueRetriever in someValueRetrievers){
                foreach(var type in valueRetriever.TypesForWhichIRetrieveValues()){
                    _valueRetrievers[type] = (TableRow row, Type targetType) => valueRetriever.ExtractValueFromRow(row, targetType);
                }
            }
        }

        public void RegisterValueComparer(IValueComparer valueComparer, string uniqueId)
        {
            _valueComparers[uniqueId] = valueComparer;
        }

        public void LoadContainer(IObjectContainer container)
        {
            foreach (var key in _valueComparers.Keys)
                container.RegisterInstanceAs<IValueComparer>(_valueComparers [key], key);
        }

    }
}

