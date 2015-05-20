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
        private IDictionary<Type, Func<TableRow, Type, object>> _valueRetrievers = new Dictionary<Type, Func<TableRow, Type, object>>();

        public static Service Instance { get; internal set; }

        public IDictionary<string, IValueComparer> ValueComparers { get { return _valueComparers; } }
        public IDictionary<Type, Func<TableRow, Type, object>> ValueRetrievers { get { return _valueRetrievers; } }

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

            _valueRetrievers[typeof(string)]    = (TableRow row, Type _) => new StringValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(byte)]      = (TableRow row, Type _) => new ByteValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(sbyte)]     = (TableRow row, Type _) => new SByteValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(int)]       = (TableRow row, Type _) => new IntValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(uint)]      = (TableRow row, Type _) => new UIntValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(short)]     = (TableRow row, Type _) => new ShortValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(ushort)]    = (TableRow row, Type _) => new UShortValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(long)]      = (TableRow row, Type _) => new LongValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(ulong)]     = (TableRow row, Type _) => new ULongValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(float)]     = (TableRow row, Type _) => new FloatValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(double)]    = (TableRow row, Type _) => new DoubleValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(decimal)]   = (TableRow row, Type _) => new DecimalValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(char)]      = (TableRow row, Type _) => new CharValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(bool)]      = (TableRow row, Type _) => new BoolValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(DateTime)]  = (TableRow row, Type _) => new DateTimeValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(Guid)]      = (TableRow row, Type _) => new GuidValueRetriever().GetValue(row[1]);
            _valueRetrievers[typeof(Enum)]      = (TableRow row, Type type) => new EnumValueRetriever().GetValue(row[1], type.GetProperties().First(x => x.Name.MatchesThisColumnName(row[0])).PropertyType);
            _valueRetrievers[typeof(Guid?)]     = (TableRow row, Type _) => new NullableGuidValueRetriever(v => new GuidValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(DateTime?)] = (TableRow row, Type _) => new NullableDateTimeValueRetriever(v => new DateTimeValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(bool?)]     = (TableRow row, Type _) => new NullableBoolValueRetriever(v => new BoolValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(char?)]     = (TableRow row, Type _) => new NullableCharValueRetriever(v => new CharValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(decimal?)]  = (TableRow row, Type _) => new NullableDecimalValueRetriever(v => new DecimalValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(double?)]   = (TableRow row, Type _) => new NullableDoubleValueRetriever(v => new DoubleValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(float?)]    = (TableRow row, Type _) => new NullableFloatValueRetriever(v => new FloatValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(ulong?)]    = (TableRow row, Type _) => new NullableULongValueRetriever(v => new ULongValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(byte?)]     = (TableRow row, Type _) => new NullableByteValueRetriever(v => new ByteValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(sbyte?)]    = (TableRow row, Type _) => new NullableSByteValueRetriever(v => new SByteValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(int?)]      = (TableRow row, Type _) => new NullableIntValueRetriever(v => new IntValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(uint?)]     = (TableRow row, Type _) => new NullableUIntValueRetriever(v => new UIntValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(short?)]    = (TableRow row, Type _) => new NullableShortValueRetriever(v => new ShortValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(ushort?)]   = (TableRow row, Type _) => new NullableUShortValueRetriever(v => new UShortValueRetriever().GetValue(v)).GetValue(row[1]);
            _valueRetrievers[typeof(long?)]     = (TableRow row, Type _) => new NullableLongValueRetriever(v => new LongValueRetriever().GetValue(v)).GetValue(row[1]);
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

