using System;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    public static class TextHelper
    {
        public static string GetText(string name, string description)
        {
            if (string.IsNullOrEmpty(description))
                return name;

            return string.Concat(name, Environment.NewLine, description);
        }
    }
}