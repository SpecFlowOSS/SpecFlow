using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Tracing
{
    public static class CodeFormattingExtensions
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
            string identifier = ToIdentifierPart(text);
            if (identifier.Length > 0 && char.IsDigit(identifier[0]))
                identifier = "_" + identifier;

            return identifier;
        }

        public static string ToIdentifierCamelCase(this string text)
        {
            string identifier = ToIdentifier(text);
            if (identifier.Length > 0)
                identifier = identifier.Substring(0, 1).ToLower() + identifier.Substring(1);

            return identifier;
        }

        static private readonly Regex firstWordCharRe = new Regex(@"(?<pre>[^\p{Ll}\p{Lu}]+)(?<fc>[\p{Ll}\p{Lu}])");
        static private readonly Regex punctCharRe = new Regex(@"[\n\.-]+");

        public static string ToIdentifierPart(this string text)
        {
            text = firstWordCharRe.Replace(text, match => match.Groups["pre"].Value + match.Groups["fc"].Value.ToUpper());

            text = punctCharRe.Replace(text, "_");

            text = RemoveAccentAndPunctuationChars(text);

            if (text.Length > 0)
                text = text.Substring(0, 1).ToUpper() + text.Substring(1);

            return text;
        }

        public static string TrimEllipse(this string text, int maxLength)
        {
            if (text == null || text.Length <= maxLength)
                return text;

            const string ellipse = "...";
            return text.Substring(0, maxLength - ellipse.Length) + ellipse;
        }

        #region Accent replacements
        static private Dictionary<string, string> accentReplacements = new Dictionary<string, string>()
                                                                {
                                                                    {"\u00C0", "A"},
                                                                    {"\u00C1", "A"},
                                                                    {"\u00C2", "A"},
                                                                    {"\u00C3", "A"},
                                                                    {"\u00C4", "A"},
                                                                    {"\u00C5", "A"},
                                                                    {"\u00C6", "AE"},
                                                                    {"\u00C7", "C"},
                                                                    {"\u00C8", "E"},
                                                                    {"\u00C9", "E"},
                                                                    {"\u00CA", "E"},
                                                                    {"\u00CB", "E"},
                                                                    {"\u00CC", "I"},
                                                                    {"\u00CD", "I"},
                                                                    {"\u00CE", "I"},
                                                                    {"\u00CF", "I"},
                                                                    {"\u00D0", "D"},
                                                                    {"\u00D1", "N"},
                                                                    {"\u00D2", "O"},
                                                                    {"\u00D3", "O"},
                                                                    {"\u00D4", "O"},
                                                                    {"\u00D5", "O"},
                                                                    {"\u00D6", "O"},
                                                                    {"\u00D8", "O"},
                                                                    {"\u00D9", "U"},
                                                                    {"\u00DA", "U"},
                                                                    {"\u00DB", "U"},
                                                                    {"\u00DC", "U"},
                                                                    {"\u00DD", "Y"},
                                                                    {"\u00DF", "B"},
                                                                    {"\u00E0", "a"},
                                                                    {"\u00E1", "a"},
                                                                    {"\u00E2", "a"},
                                                                    {"\u00E3", "a"},
                                                                    {"\u00E4", "a"},
                                                                    {"\u00E5", "a"},
                                                                    {"\u00E6", "ae"},
                                                                    {"\u00E7", "c"},
                                                                    {"\u00E8", "e"},
                                                                    {"\u00E9", "e"},
                                                                    {"\u00EA", "e"},
                                                                    {"\u00EB", "e"},
                                                                    {"\u00EC", "i"},
                                                                    {"\u00ED", "i"},
                                                                    {"\u00EE", "i"},
                                                                    {"\u00EF", "i"},
                                                                    //{"\u00F0", "d"},
                                                                    {"\u00F1", "n"},
                                                                    {"\u00F2", "o"},
                                                                    {"\u00F3", "o"},
                                                                    {"\u00F4", "o"},
                                                                    {"\u00F5", "o"},
                                                                    {"\u00F6", "o"},
                                                                    {"\u00F8", "o"},
                                                                    {"\u00F9", "u"},
                                                                    {"\u00FA", "u"},
                                                                    {"\u00FB", "u"},
                                                                    {"\u00FC", "u"},
                                                                    {"\u00FD", "y"},
                                                                    {"\u00FF", "y"},


                                                                    {"\u0104", "A"},
                                                                    {"\u0141", "L"},
                                                                    {"\u013D", "L"},
                                                                    {"\u015A", "S"},
                                                                    {"\u0160", "S"},
                                                                    {"\u015E", "S"},
                                                                    {"\u0164", "T"},
                                                                    {"\u0179", "Z"},
                                                                    {"\u017D", "Z"},
                                                                    {"\u017B", "Z"},
                                                                    {"\u0105", "a"},
                                                                    {"\u0142", "l"},
                                                                    {"\u013E", "l"},
                                                                    {"\u015B", "s"},
                                                                    {"\u0161", "s"},
                                                                    {"\u015F", "s"},
                                                                    {"\u0165", "t"},
                                                                    {"\u017A", "z"},
                                                                    {"\u017E", "z"},
                                                                    {"\u017C", "z"},
                                                                    {"\u0154", "R"},
                                                                    {"\u0102", "A"},
                                                                    {"\u0139", "L"},
                                                                    {"\u0106", "C"},
                                                                    {"\u010C", "C"},
                                                                    {"\u0118", "E"},
                                                                    {"\u011A", "E"},
                                                                    {"\u010E", "D"},
                                                                    {"\u0110", "D"},
                                                                    {"\u0143", "N"},
                                                                    {"\u0147", "N"},
                                                                    {"\u0150", "O"},
                                                                    {"\u0158", "R"},
                                                                    {"\u016E", "U"},
                                                                    {"\u0170", "U"},
                                                                    {"\u0162", "T"},
                                                                    {"\u0155", "r"},
                                                                    {"\u0103", "a"},
                                                                    {"\u013A", "l"},
                                                                    {"\u0107", "c"},
                                                                    {"\u010D", "c"},
                                                                    {"\u0119", "e"},
                                                                    {"\u011B", "e"},
                                                                    {"\u010F", "d"},
                                                                    {"\u0111", "d"},
                                                                    {"\u0144", "n"},
                                                                    {"\u0148", "n"},
                                                                    {"\u0151", "o"},
                                                                    {"\u0159", "r"},
                                                                    {"\u016F", "u"},
                                                                    {"\u0171", "u"},
                                                                    {"\u0163", "t"},
                                                                };
        #endregion

        static private readonly Regex nonIdentifierRe = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Nd}\p{Pc}]");
        static private readonly Regex nonLatinRe = new Regex("[^a-zA-Z]");

        public static string RemoveAccentAndPunctuationChars(string text)
        {
            var nonIdRemoved = nonIdentifierRe.Replace(text, String.Empty);

            return nonLatinRe.Replace(nonIdRemoved, match =>
               {
                   string result;
                   // if there is a Latin substitute, we use that
                   if (accentReplacements.TryGetValue(match.Value, out result))
                       return result;
                   return match.Value;
               });
        }
    }
}