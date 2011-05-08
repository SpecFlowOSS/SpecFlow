namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class CharValueRetriever
    {
        public virtual char GetValue(string value)
        {
            return ThisStringIsNotASingleCharacter(value)
                       ? '\0'
                       : value[0];
        }

        private bool ThisStringIsNotASingleCharacter(string value)
        {
            return string.IsNullOrEmpty(value) || value.Length > 1;
        }
    }
}