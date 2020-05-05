using System;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities
{
    class PersonWithMandatoryLastName : Person
    {
        /// <inheritdoc />
        public PersonWithMandatoryLastName(string lastName)
        {
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        }
    }
}
