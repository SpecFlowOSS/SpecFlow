using System;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities
{
    public class Person
    {
        public Sex Sex { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int NumberOfIdeas { get; set; }
        public decimal Salary { get; set; }
        public bool IsRational { get; set; }
    }
}