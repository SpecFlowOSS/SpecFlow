using System;
using System.Collections.Generic;


namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities
{
    public class Person
    {
        public Sex Sex { get; set; }
        public string FirstName { get; set; }
        public char MiddleInitial { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int NumberOfIdeas { get; set; }
        public decimal Salary { get; set; }
        public bool IsRational { get; set; }

        public char? NullableChar { get; set; }

        public DateTime? NullableDateTime { get; set; }
        public bool? NullableBool { get; set; }

        public decimal? NullableDecimal { get; set; }
        public int? NullableInt { get; set; }

        public short Short { get; set; }
        public short? NullableShort { get; set; }

        public ushort UShort { get; set; }
        public ushort? NullableUShort { get; set; }

        public long Long { get; set; }
        public long? NullableLong { get; set; }

        public ulong ULong { get; set; }
        public ulong? NullableULong { get; set; }

        public double Double { get; set; }
        public double? NullableDouble { get; set; }

        public Guid GuidId { get; set; }
        public Guid? NullableGuidId { get; set; }

        public byte Byte { get; set; }
        public byte? NullableByte { get; set; }

        public sbyte SByte { get; set; }
        public sbyte? NullableSByte { get; set; }

        public float Float { get; set; }
        public float? NullableFloat { get; set; }
        public uint UnsignedInt { get; set; }
        public uint? NullableUnsignedInt { get; set; }
        public string With_Underscore { get; set; }

        public string WithUmlauteäöü { get; set; }

        public string[] StringArray { get; set; }
        public List<string> StringList { get; set; }
        public Language[] Languages { get; set; }
        public IList<Language> LanguageList { get; set; }

    }
}