using System;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    /// <summary>
    /// not required in the final AST
    /// </summary>
    public class Text
    {
        public string Value { get; set; }

        public Text(params string[] textElements)
        {
            if (textElements != null && textElements.Length > 0)
                Value = string.Concat(textElements);
            else
                Value = String.Empty;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}