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
        private Dictionary<string, IValueRetriever> _valueRetrievers = new Dictionary<string, IValueRetriever>();

        public static Service Instance { get; internal set; }

        public IDictionary<string, IValueComparer> ValueComparers { get { return _valueComparers; } }

        public IDictionary<string, IValueRetriever> ValueRetrievers { get { return _valueRetrievers; } }

        public IDictionary<Type, Func<TableRow, Type, object>> ValueRetrieversByType {
            get {
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
                    new NullableGuidValueRetriever(),
                    new NullableDateTimeValueRetriever(),
                    new NullableBoolValueRetriever(),
                    new NullableCharValueRetriever(),
                    new NullableDecimalValueRetriever(),
                    new NullableDoubleValueRetriever(),
                    new NullableFloatValueRetriever(),
                    new NullableULongValueRetriever(),
                    new NullableByteValueRetriever(),
                    new NullableSByteValueRetriever(),
                    new NullableIntValueRetriever(),
                    new NullableUIntValueRetriever(),
                    new NullableShortValueRetriever(),
                    new NullableUShortValueRetriever(),
                    new NullableLongValueRetriever(),
                };

                var result = new Dictionary<Type, Func<TableRow, Type, object>>();
                foreach(var valueRetriever in someValueRetrievers){
                    foreach(var type in valueRetriever.TypesForWhichIRetrieveValues()){
                        result[type] = (TableRow row, Type targetType) => valueRetriever.ExtractValueFromRow(row, targetType);
                    }
                }
                return result; 
            }
        }

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

