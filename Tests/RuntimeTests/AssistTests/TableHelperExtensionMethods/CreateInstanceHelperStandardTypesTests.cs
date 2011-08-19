using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    [TestFixture]
    public class CreateInstanceHelperStandardTypesTests
    {
        [SetUp]
        public void SetUp()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        [Test]
        public void Sets_byte_value()
        {
            SetAndCompareValue<byte>("ByteProperty", 1, x => x.ByteProperty);
        }

        [Test]
        public void Sets_nullable_byte_value()
        {
            SetAndCompareValue<byte?>("NullableByteProperty", 2, x => x.NullableByteProperty);
        }

        [Test]
        public void Sets_sbyte_value()
        {
            SetAndCompareValue<sbyte>("SByteProperty", -3, x => x.SByteProperty);
        }

        [Test]
        public void Sets_nullable_sbyte_value()
        {
            SetAndCompareValue<sbyte?>("NullableSByteProperty", -4, x => x.NullableSByteProperty);
        }

        [Test]
        public void Sets_int_value()
        {
            SetAndCompareValue<int>("IntProperty", -5, x => x.IntProperty);
        }

        [Test]
        public void Sets_nullable_int_value()
        {
            SetAndCompareValue<int?>("NullableIntProperty", -6, x => x.NullableIntProperty);
        }

        [Test]
        public void Sets_uint_value()
        {
            SetAndCompareValue<uint>("UIntProperty", 7, x => x.UIntProperty);
        }

        [Test]
        public void Sets_nullable_uint_value()
        {
            SetAndCompareValue<uint?>("NullableUIntProperty", 8, x => x.NullableUIntProperty);
        }

        [Test]
        public void Sets_short_value()
        {
            SetAndCompareValue<short>("ShortProperty", -9, x => x.ShortProperty);
        }

        [Test]
        public void Sets_nullable_short_value()
        {
            SetAndCompareValue<short?>("NullableShortProperty", -10, x => x.NullableShortProperty);
        }

        [Test]
        public void Sets_ushort_value()
        {
            SetAndCompareValue<ushort>("UShortProperty", 11, x => x.UShortProperty);
        }

        [Test]
        public void Sets_nullable_ushort_value()
        {
            SetAndCompareValue<ushort?>("NullableUShortProperty", 12, x => x.NullableUShortProperty);
        }

        [Test]
        public void Sets_long_value()
        {
            SetAndCompareValue<long>("LongProperty", -13, x => x.LongProperty);
        }

        [Test]
        public void Sets_nullable_long_value()
        {
            SetAndCompareValue<long?>("NullableLongProperty", -14, x => x.NullableLongProperty);
        }

        [Test]
        public void Sets_ulong_value()
        {
            SetAndCompareValue<ulong>("ULongProperty", 15, x => x.ULongProperty);
        }

        [Test]
        public void Sets_nullable_ulong_value()
        {
            SetAndCompareValue<ulong?>("NullableULongProperty", 16, x => x.NullableULongProperty);
        }

        [Test]
        public void Sets_float_value()
        {
            SetAndCompareValue<float>("FloatProperty", 1.01f, x => x.FloatProperty);
        }

        [Test]
        public void Sets_nullable_float_value()
        {
            SetAndCompareValue<float?>("NullableFloatProperty", 2.02f, x => x.NullableFloatProperty);
        }

        [Test]
        public void Sets_double_value()
        {
            SetAndCompareValue<double>("DoubleProperty", 3.03, x => x.DoubleProperty);
        }

        [Test]
        public void Sets_nullable_double_value()
        {
            SetAndCompareValue<double?>("NullableDoubleProperty", 4.04, x => x.NullableDoubleProperty);
        }

        [Test]
        public void Sets_decimal_value()
        {
            SetAndCompareValue<decimal>("DecimalProperty", 5.05m, x => x.DecimalProperty);
        }

        [Test]
        public void Sets_nullable_decimal_value()
        {
            SetAndCompareValue<decimal?>("NullableDecimalProperty", 6.06m, x => x.NullableDecimalProperty);
        }

        [Test]
        public void Sets_char_value()
        {
            SetAndCompareValue<char>("CharProperty", 'a', x => x.CharProperty);
        }

        [Test]
        public void Sets_nullable_char_value()
        {
            SetAndCompareValue<char?>("NullableCharProperty", 'b', x => x.NullableCharProperty);
        }

        [Test]
        public void Sets_bool_value()
        {
            SetAndCompareValue<bool>("BoolProperty", true, x => x.BoolProperty);
        }

        [Test]
        public void Sets_nullable_bool_value()
        {
            SetAndCompareValue<bool?>("NullableBoolProperty", false, x => x.NullableBoolProperty);
        }

        [Test]
        public void Sets_datetime_value()
        {
            SetAndCompareValue<DateTime>("DateTimeProperty", new DateTime(2011, 1, 1), x => x.DateTimeProperty);
        }

        [Test]
        public void Sets_nullable_datetime_value()
        {
            SetAndCompareValue<DateTime?>("NullableDateTimeProperty", new DateTime(2022, 2, 2), x => x.NullableDateTimeProperty);
        }

        [Test]
        public void Sets_guid_value()
        {
            SetAndCompareValue<Guid>("GuidProperty", Guid.NewGuid(), x => x.GuidProperty);
        }

        [Test]
        public void Sets_nullable_guid_value()
        {
            SetAndCompareValue<Guid?>("NullableGuidProperty", Guid.NewGuid(), x => x.NullableGuidProperty);
        }

        [Test]
        public void Sets_string_value()
        {
            SetAndCompareValue<string>("StringProperty", "Test", x => x.StringProperty);
        }

        private void SetAndCompareValue<T>(string propertyName, T value, Func<StandardTypesComparisonTestObject, T> propertyFunc)
        {
            var table = new Table("Field", "Value");
            table.AddRow(propertyName, value.ToString());

            var testObject = table.CreateInstance<StandardTypesComparisonTestObject>();

            propertyFunc(testObject).ShouldEqual(value);
        }
    }
}
