using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow
{
    internal static class StringExtensions
    {
        public static string Indent(this string text, string indent)
        {
            if (text.EndsWith(Environment.NewLine))
            {
                return text.Remove(text.Length - Environment.NewLine.Length).Indent(indent) + Environment.NewLine;
            }

            return indent + text.Replace(Environment.NewLine, Environment.NewLine + indent);
        }

        public static string ToIdentifier(this string text)
        {
            Regex firstWordCharRe = new Regex(@"(?<pre>[^\p{Ll}\p{Lu}]+)(?<fc>[\p{Ll}\p{Lu}])");
            text = firstWordCharRe.Replace(text, match => match.Groups["pre"].Value + match.Groups["fc"].Value.ToUpper());

            Regex punctCharRe = new Regex(@"[\n\.-]+");
            text = punctCharRe.Replace(text, "_");

            Regex nonWordCharRe = new Regex(@"[^a-zA-Z0-9_]+");
            text = nonWordCharRe.Replace(text, "");

            if (text.Length > 0)
            {
                text = text.Substring(0, 1).ToUpper() + text.Substring(1);

                if (char.IsDigit(text[0]))
                    text = "_" + text;
            }

            return text;
        }

        public static string ToIdentifierCamelCase(this string text)
        {
            string identifier = ToIdentifier(text);
            if (text.Length > 0)
                identifier = identifier.Substring(0, 1).ToLower() + identifier.Substring(1);

            return identifier;
        }

        public static string TrimEllipse(this string text, int maxLength)
        {
            if (text == null || text.Length <= maxLength)
                return text;

            const string ellipse = "...";
            return text.Substring(0, maxLength - ellipse.Length) + ellipse;
        }
    }
}