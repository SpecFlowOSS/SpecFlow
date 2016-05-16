using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    using System.Linq;

    public static class ParameterNameExtensions
    {
        public static bool IsSingleWordWithNoNumbers(this string value)
        {
            return Regex.IsMatch(value, @"^[a-zA-Z]+$");
        }

        public static bool IsSingleWordSurroundedByAngleBrackets(this string value)
        {
            if (value.StartsWith("<") && value.EndsWith(">"))
            {
                return value.Substring(1, value.Length - 2).IsSingleWordWithNoNumbers();
            }
            return false;
        }

        public static string WithoutSurroundingAngleBrackets(this string value)
        {
            return value.Substring(1, value.Length - 2);
        }

        public static string WithLowerCaseFirstLetter(this string value)
        {
            return value.Substring(0,1).ToLower() +  value.Substring(1);
        }

        public static string UniquelyIdentified(this string value, List<string> usedParameterNames, int parmIndex)
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