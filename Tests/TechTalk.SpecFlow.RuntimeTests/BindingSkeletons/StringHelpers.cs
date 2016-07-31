using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.RuntimeTests.BindingSkeletons
{
    internal static class StringHelpers
    {
        static private readonly Regex lineEndingRe = new Regex(@"\r?\n");
        public static string ConsolidateVerbatimStringLineEndings(string message)
        {
            return lineEndingRe.Replace(message, Environment.NewLine);
        }
    }
}
