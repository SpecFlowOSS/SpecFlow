using TechTalk.SpecFlow.Assist.Attributes;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure
{
    public class AliasedTestObject
    {
        [TableAliases("Alias[ ]*One", "First[ ]?Name", "^Known As$")]
        public string PropertyOne { get; set; }

        [TableAliases("Alias[ ]*Two", "Middle[ ]?Name", "^Never Known As$")]
        public string PropertyTwo { get; set; }

        [TableAliases("AliasThree")]
        [TableAliases("Surname")]
        [TableAliases("Last[ ]?name")]
        [TableAliases("Dad's Last Name")]
        public string PropertyThree { get; set; }

#pragma warning disable 649
        [TableAliases("FieldAliasOne")]
        public string FieldOne;
        [TableAliases("FieldAliasTwo")]
        public string FieldTwo;
        [TableAliases("FieldAliasThree")]
        public string FieldThree;
#pragma warning restore 649

        [TableAliases("AliasOne")]
        public string AnotherPropertyWithSameAlias { get; set; }

        [TableAliases("AliasOne")]
        public long AliasedButTypeMismatch { get; set; }
    }
}
