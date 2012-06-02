using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Configuration
{
    public class ValueRetrieverCollection
    {
        internal static IValueRetriever<T> GetValueRetriever<T>()
        {
            return (IValueRetriever<T>)ValueRetrievers[typeof(T)]();
        }

        internal static IEnumValueRetriever GetEnumValueRetriever()
        {
            return (IEnumValueRetriever)ValueRetrievers[typeof(Enum)]();
        }

        private static readonly Dictionary<Type, Func<IValueRetriever>> valueRetrievers = new Dictionary<Type, Func<IValueRetriever>>
            {
                {typeof(string), () => new StringValueRetriever()},
                {typeof (byte), () => new ByteValueRetriever()},
                {typeof (byte?), () => new NullableByteValueRetriever(GetValueRetriever<byte>())},
                {typeof (sbyte), () => new SByteValueRetriever()},
                {typeof (sbyte?), () => new NullableSByteValueRetriever(GetValueRetriever<sbyte>())},
                {typeof (int), () => new IntValueRetriever()},
                {typeof (int?), () => new NullableIntValueRetriever(GetValueRetriever<int>())},
                {typeof (uint), () => new UIntValueRetriever()},
                {typeof (uint?), () => new NullableUIntValueRetriever(GetValueRetriever<uint>())},
                {typeof (short), () => new ShortValueRetriever()},
                {typeof (short?), () => new NullableShortValueRetriever(GetValueRetriever<short>())},
                {typeof (ushort), () => new UShortValueRetriever()},
                {typeof (ushort?), () => new NullableUShortValueRetriever(GetValueRetriever<ushort>())},
                {typeof (long), () => new LongValueRetriever()},
                {typeof (long?), () => new NullableLongValueRetriever(GetValueRetriever<long>())},
                {typeof (ulong), () => new ULongValueRetriever()},
                {typeof (ulong?), () => new NullableULongValueRetriever(GetValueRetriever<ulong>())},
                {typeof (float), () => new FloatValueRetriever()},
                {typeof (float?), () => new NullableFloatValueRetriever(GetValueRetriever<float>())},
                {typeof (double), () => new DoubleValueRetriever()},
                {typeof (double?), () => new NullableDoubleValueRetriever(GetValueRetriever<double>())},
                {typeof (decimal), () => new DecimalValueRetriever()},
                {typeof (decimal?), () => new NullableDecimalValueRetriever(GetValueRetriever<decimal>())},
                {typeof (char), () => new CharValueRetriever()},
                {typeof (char?), () => new NullableCharValueRetriever(GetValueRetriever<char>())},
                {typeof (bool), () => new BoolValueRetriever()},
                {typeof (bool?), () => new NullableBoolValueRetriever(GetValueRetriever<bool>())},
                {typeof (DateTime), () => new DateTimeValueRetriever()},
                {typeof (DateTime?), () => new NullableDateTimeValueRetriever(GetValueRetriever<DateTime>())},
                {typeof (Guid), () => new GuidValueRetriever()},
                {typeof (Guid?), () => new NullableGuidValueRetriever(GetValueRetriever<Guid>())},
                {typeof (Enum), () => new EnumValueRetriever()}
            };

        public static Dictionary<Type, Func<IValueRetriever>> ValueRetrievers { get { return valueRetrievers; } }
    }
}
