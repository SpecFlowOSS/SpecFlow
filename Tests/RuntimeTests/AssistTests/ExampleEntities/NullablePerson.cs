﻿using System;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities
{
    public class NullablePerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? NumberOfIdeas { get; set; }
        public decimal? Salary { get; set; }
        public bool? IsRational { get; set; }
        public double? NullableDouble { get; set; }
        public Guid? NullableGuid { get; set; }
        public char? NullableChar { get; set; }
        public Single? NullableSingle { get; set; }
    }
}