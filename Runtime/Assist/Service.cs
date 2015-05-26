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

        private Dictionary<string, IValueComparer> _registeredValueComparers = new Dictionary<string, IValueComparer>();
        private Dictionary<string, IValueRetriever> _registeredValueRetrievers = new Dictionary<string, IValueRetriever>();

        public static Service Instance { get; internal set; }

        public IDictionary<string, IValueComparer> RegisteredValueComparers { get { return _registeredValueComparers; } }

        public IDictionary<string, IValueRetriever> RegisteredValueRetrievers { get { return _registeredValueRetrievers; } }

        public IDictionary<Type, Func<TableRow, Type, object>> ValueRetrieversByType {
            get {
                var result = new Dictionary<Type, Func<TableRow, Type, object>>();
                foreach(var valueRetriever in _registeredValueRetrievers.Values){
                    foreach(var type in valueRetriever.TypesForWhichIRetrieveValues()){
                        result[type] = (TableRow row, Type targetType) => valueRetriever.ExtractValueFromRow(row, targetType);
                    }
                }
                return result; 
            }
        }

        public IEnumerable<IValueComparer> ValueComparers { get { return _registeredValueComparers.Values; } }

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

            RegisterValueRetriever(new StringValueRetriever(), "vstring");
            RegisterValueRetriever(new ByteValueRetriever(), "vbyte");
            RegisterValueRetriever(new SByteValueRetriever(), "vsbyte");
            RegisterValueRetriever(new IntValueRetriever(), "vint");
            RegisterValueRetriever(new UIntValueRetriever(), "vuint");
            RegisterValueRetriever(new ShortValueRetriever(), "vshort");
            RegisterValueRetriever(new UShortValueRetriever(), "vushort");
            RegisterValueRetriever(new LongValueRetriever(), "vlong");
            RegisterValueRetriever(new ULongValueRetriever(), "vulong");
            RegisterValueRetriever(new FloatValueRetriever(), "vfloat");
            RegisterValueRetriever(new DoubleValueRetriever(), "vdouble");
            RegisterValueRetriever(new DecimalValueRetriever(), "vdecimal");
            RegisterValueRetriever(new CharValueRetriever(), "vchar");
            RegisterValueRetriever(new BoolValueRetriever(), "vbool");
            RegisterValueRetriever(new DateTimeValueRetriever(), "vdatetime");
            RegisterValueRetriever(new GuidValueRetriever(), "vguid");
            RegisterValueRetriever(new EnumValueRetriever(), "venum");
            RegisterValueRetriever(new NullableGuidValueRetriever(), "vnullableguid");
            RegisterValueRetriever(new NullableDateTimeValueRetriever(), "vnullabledatetime");
            RegisterValueRetriever(new NullableBoolValueRetriever(), "vnullablebool");
            RegisterValueRetriever(new NullableCharValueRetriever(), "vnullablechar");
            RegisterValueRetriever(new NullableDecimalValueRetriever(), "vnullabledecimal");
            RegisterValueRetriever(new NullableDoubleValueRetriever(), "vnullabledouble");
            RegisterValueRetriever(new NullableFloatValueRetriever(), "vnullablefloat");
            RegisterValueRetriever(new NullableULongValueRetriever(), "vnullableulong");
            RegisterValueRetriever(new NullableByteValueRetriever(), "vnullablebyte");
            RegisterValueRetriever(new NullableSByteValueRetriever(), "vnullablesbyte");
            RegisterValueRetriever(new NullableIntValueRetriever(), "vnullableint");
            RegisterValueRetriever(new NullableUIntValueRetriever(), "vnullableuint");
            RegisterValueRetriever(new NullableShortValueRetriever(), "vnullableshort");
            RegisterValueRetriever(new NullableUShortValueRetriever(), "vnullableushort");
            RegisterValueRetriever(new NullableLongValueRetriever(), "vnullablelong");
        }

        public void RegisterValueComparer(IValueComparer valueComparer, string uniqueId)
        {
            _registeredValueComparers[uniqueId] = valueComparer;
        }

        public void RegisterValueRetriever(IValueRetriever valueRetriever, string uniqueId)
        {
            _registeredValueRetrievers[uniqueId] = valueRetriever;
        }

        public void LoadContainer(IObjectContainer container)
        {
            foreach (var key in _registeredValueComparers.Keys)
                container.RegisterInstanceAs<IValueComparer>(_registeredValueComparers[key], key);
            foreach (var key in _registeredValueRetrievers.Keys)
                container.RegisterInstanceAs<IValueRetriever>(_registeredValueRetrievers[key], key);
        }

    }
}

