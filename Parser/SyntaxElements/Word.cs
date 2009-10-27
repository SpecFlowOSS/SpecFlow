using System;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    /// <summary>
    /// not required in the final AST
    /// </summary>
    public class Word
    {
        public string Value { get; set; }

        public Word(string value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}