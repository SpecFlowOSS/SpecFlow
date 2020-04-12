using System;
using System.Globalization;
using System.Threading;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    
    public class CreateInstanceHelperStandardTypesTests
    {
        public CreateInstanceHelperStandardTypesTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Fact]
        public void Sets_byte_value()
        {
            SetAndCompareValue<byte>("ByteProperty", 1, x => x.ByteProperty);
        }

        [Fact]
        public void Sets_nullable_byte_value()
        {
            SetAndCompareValue<byte?>("NullableByteProperty", 2, x => x.NullableByteProperty);
        }

        [Fact]
        public void Sets_sbyte_value()
        {
            SetAndCompareValue<sbyte>("SByteProperty", -3, x => x.SByteProperty);
        }

        [Fact]
        public void Sets_nullable_sbyte_value()
        {
            SetAndCompareValue<sbyte?>("NullableSByteProperty", -4, x => x.NullableSByteProperty);
        }

        [Fact]
        public void Sets_int_value()
        {
            SetAndCompareValue<int>("IntProperty", -5, x => x.IntProperty);
        }

        [Fact]
        public void Sets_nullable_int_value()
        {
            SetAndCompareValue<int?>("NullableIntProperty", -6, x => x.NullableIntProperty);
        }

        [Fact]
        public void Sets_uint_value()
        {
            SetAndCompareValue<uint>("UIntProperty", 7, x => x.UIntProperty);
        }

        [Fact]
        public void Sets_nullable_uint_value()
        {
            SetAndCompareValue<uint?>("NullableUIntProperty", 8, x => x.NullableUIntProperty);
        }

        [Fact]
        public void Sets_short_value()
        {
            SetAndCompareValue<short>("ShortProperty", -9, x => x.ShortProperty);
        }

        [Fact]
        public void Sets_nullable_short_value()
        {
            SetAndCompareValue<short?>("NullableShortProperty", -10, x => x.NullableShortProperty);
        }

        [Fact]
        public void Sets_ushort_value()
        {
            SetAndCompareValue<ushort>("UShortProperty", 11, x => x.UShortProperty);
        }

        [Fact]
        public void Sets_nullable_ushort_value()
        {
            SetAndCompareValue<ushort?>("NullableUShortProperty", 12, x => x.NullableUShortProperty);
        }

        [Fact]
        public void Sets_long_value()
        {
            SetAndCompareValue<long>("LongProperty", -13, x => x.LongProperty);
        }

        [Fact]
        public void Sets_nullable_long_value()
        {
            SetAndCompareValue<long?>("NullableLongProperty", -14, x => x.NullableLongProperty);
        }

        [Fact]
        public void Sets_ulong_value()
        {
            SetAndCompareValue<ulong>("ULongProperty", 15, x => x.ULongProperty);
        }

        [Fact]
        public void Sets_nullable_ulong_value()
        {
            SetAndCompareValue<ulong?>("NullableULongProperty", 16, x => x.NullableULongProperty);
        }

        [Fact]
        public void Sets_float_value()
        {
            SetAndCompareValue<float>("FloatProperty", 1.01f, x => x.FloatProperty);
        }

        [Fact]
        public void Sets_nullable_float_value()
        {
            SetAndCompareValue<float?>("NullableFloatProperty", 2.02f, x => x.NullableFloatProperty);
        }

        [Fact]
        public void Sets_double_value()
        {
            SetAndCompareValue<double>("DoubleProperty", 3.03, x => x.DoubleProperty);
        }

        [Fact]
        public void Sets_nullable_double_value()
        {
            SetAndCompareValue<double?>("NullableDoubleProperty", 4.04, x => x.NullableDoubleProperty);
        }

        [Fact]
        public void Sets_decimal_value()
        {
            SetAndCompareValue<decimal>("DecimalProperty", 5.05m, x => x.DecimalProperty);
        }

        [Fact]
        public void Sets_nullable_decimal_value()
        {
            SetAndCompareValue<decimal?>("NullableDecimalProperty", 6.06m, x => x.NullableDecimalProperty);
        }

        [Fact]
        public void Sets_char_value()
        {
            SetAndCompareValue<char>("CharProperty", 'a', x => x.CharProperty);
        }

        [Fact]
        public void Sets_nullable_char_value()
        {
            SetAndCompareValue<char?>("NullableCharProperty", 'b', x => x.NullableCharProperty);
        }

        [Fact]
        public void Sets_bool_value()
        {
            SetAndCompareValue<bool>("BoolProperty", true, x => x.BoolProperty);
        }

        [Fact]
        public void Sets_nullable_bool_value()
        {
            SetAndCompareValue<bool?>("NullableBoolProperty", false, x => x.NullableBoolProperty);
        }

        [Fact]
        public void Sets_datetime_value()
        {
            SetAndCompareValue<DateTime>("DateTimeProperty", new DateTime(2011, 1, 1), x => x.DateTimeProperty);
        }

        [Fact]
        public void Sets_nullable_datetime_value()
        {
            SetAndCompareValue<DateTime?>("NullableDateTimeProperty", new DateTime(2022, 2, 2), x => x.NullableDateTimeProperty);
        }

        [Fact]
        public void Sets_guid_value()
        {
            SetAndCompareValue<Guid>("GuidProperty", Guid.NewGuid(), x => x.GuidProperty);
        }

        [Fact]
        public void Sets_nullable_guid_value()
        {
            SetAndCompareValue<Guid?>("NullableGuidProperty", Guid.NewGuid(), x => x.NullableGuidProperty);
        }

        [Fact]
        public void Sets_string_value()
        {
            SetAndCompareValue<string>("StringProperty", "Test", x => x.StringProperty);
        }

        private void SetAndCompareValue<T>(string propertyName, T value, Func<StandardTypesComparisonTestObject, T> propertyFunc)
        {
            var table = new Table("Field", "Value");
            table.AddRow(propertyName, value.ToString());

            var testObject = table.CreateInstance<StandardTypesComparisonTestObject>();

            propertyFunc(testObject).Should().Be(value);
        }
    }
}
