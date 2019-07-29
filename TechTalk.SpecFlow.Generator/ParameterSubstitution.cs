using System.Collections.Generic;

namespace TechTalk.SpecFlow.Generator
{
    public class ParameterSubstitution : List<KeyValuePair<string, string>>
    {
        public void Add(string parameter, string identifier)
        {
            Add(new KeyValuePair<string, string>(parameter.Trim(), identifier));
        }

        public bool TryGetIdentifier(string param, out string id)
        {
            param = param.Trim();
            foreach (var pair in this)
            {
                if (pair.Key.Equals(param))
                {
                    id = pair.Value;
                    return true;
                }
            }

            id = null;
            return false;
        }
    }
}