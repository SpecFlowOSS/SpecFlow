using System.Collections.Generic;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    using System.Text.RegularExpressions;

    public class StepParameterNameGenerator
    {
        public static  string GenerateParameterName(string value,int paramIndex, List<string> usedParameterNames)
        {
            if (IsSingleWordSurroundedByAngleBrackets(value))
                value = RemoveSurroundingAngleBrackets(value);
            if (IsSingleWordWithNoNumbers(value))
                return UniquelyIdentified(LowerCaseFirstLetter(value), usedParameterNames, paramIndex);
            return "p" + paramIndex;
        }

        public static bool IsSingleWordWithNoNumbers(string value)
        {
            return Regex.IsMatch(value, @"^[\p{L}]+$");
        }

        public static bool IsSingleWordSurroundedByAngleBrackets(string value)
        {
            if (value.StartsWith("<") && value.EndsWith(">"))
            {
                return IsSingleWordWithNoNumbers(value.Substring(1, value.Length - 2));
            }
            return false;
        }

        public static string RemoveSurroundingAngleBrackets(string value)
        {
            return value.Substring(1, value.Length - 2);
        }

        public static string LowerCaseFirstLetter(string value)
        {
            return value.Substring(0, 1).ToLower() + value.Substring(1);
        }

        public static string UniquelyIdentified(string value, List<string> usedParameterNames, int parmIndex)
        {
            if (usedParameterNames.Contains(value))
            {
                return value + parmIndex;
            }
            usedParameterNames.Add(value);
            return value;
        }
    }
}