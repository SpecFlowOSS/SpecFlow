using System;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure
{
    public class StandardTypesComparisonTestObject
    {
        public byte ByteProperty { get; set; }
        public byte? NullableByteProperty { get; set; }
        public sbyte SByteProperty { get; set; }
        public sbyte? NullableSByteProperty { get; set; }

        public int IntProperty { get; set; }
        public int? NullableIntProperty { get; set; }
        public uint UIntProperty { get; set; }
        public uint? NullableUIntProperty { get; set; }

        public short ShortProperty { get; set; }
        public short? NullableShortProperty { get; set; }
        public ushort UShortProperty { get; set; }
        public ushort? NullableUShortProperty { get; set; }

        public long LongProperty { get; set; }
        public long? NullableLongProperty { get; set; }
        public ulong ULongProperty { get; set; }
        public ulong? NullableULongProperty { get; set; }

        public float FloatProperty { get; set; }
        public float? NullableFloatProperty { get; set; }

        public double DoubleProperty { get; set; }
        public double? NullableDoubleProperty { get; set; }

        public decimal DecimalProperty { get; set; }
        public decimal? NullableDecimalProperty { get; set; }

        public char CharProperty { get; set; }
        public char? NullableCharProperty { get; set; }

        public bool BoolProperty { get; set; }
        public bool? NullableBoolProperty { get; set; }

        public DateTime DateTimeProperty { get; set; }
        public DateTime? NullableDateTimeProperty { get; set; }

        public Guid GuidProperty { get; set; }
        public Guid? NullableGuidProperty { get; set; }

        public string StringProperty { get; set; }
    }
}
