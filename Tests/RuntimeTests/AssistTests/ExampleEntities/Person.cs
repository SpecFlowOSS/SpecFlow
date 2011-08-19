using System;

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

        public double Double { get; set; }
        public double? NullableDouble { get; set; }

        public Guid GuidId { get; set; }
        public Guid? NullableGuidId { get; set; }

        public float Float { get; set; }
        public float? NullableFloat { get; set; }
        public uint UnsignedInt { get; set; }
        public uint? NullableUnsignedInt { get; set; }
    }
}