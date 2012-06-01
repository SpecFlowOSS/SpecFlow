using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities
{
    public class CustomPerson
    {
        public string Name { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
    }

    public class PhoneNumber
    {
        public int CountryCode { get; set; }
        public int AreaCode { get; set; }
        public string Phone { get; set; }
    }
}
