using System;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    /// <summary>
    /// not required in the final AST
    /// </summary>
    public class MultilineText
    {
        public string Value;

        public MultilineText(string text, string indent)
        {
            Value = (Environment.NewLine + text).Replace(Environment.NewLine + indent, Environment.NewLine).Remove(0, Environment.NewLine.Length);
        }
    }
}