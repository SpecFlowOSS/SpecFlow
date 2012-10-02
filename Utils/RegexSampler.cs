using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Utils
{
    public static class RegexSampler
    {
        private static readonly Regex reGroupStart = new Regex(@"\(");
        private static readonly Regex reGroupEnd = new Regex(@"\(");
        private static readonly Regex reChoiceRe = new Regex(@"(\([^\)]+\)\|)+\([^\)]+\)");
        private static readonly Regex reParamRe = new Regex(@"\([^\)]+\)");

        public static string GetRegexSample(string regex, string[] paramNames)
        {
            int currentParamIndex = 0;
            return GetRegexSampleInternal(regex, paramNames, false, ref currentParamIndex);
        }

        public static string GetRegexSampleInternal(string regex, string[] paramNames, bool allowEmpty, ref int currentParamIndex)
        {
            const string defaultParamPlaceholder = "<param>";

            StringBuilder regexSample = new StringBuilder();

            int pos = 0;
            int nesting = 0;
            int groupStart = 0;
            bool inNonCaptureGroup = false;
            int lastAppendedLength = 0;
            while (pos < regex.Length)
            {
                var ch = regex[pos];
                switch (ch)
                {
                    case '(':
                        if (nesting++ == 0)
                        {
                            groupStart = pos;
                            inNonCaptureGroup = regex.Length > pos + 2 && regex[pos + 1] == '?' &&
                                                regex[pos + 2] == ':';
                        }
                        break;
                    case ')':
                        if (--nesting == 0)
                        {
                            if (inNonCaptureGroup)
                            {
                                var content = regex.Substring(groupStart + 3, pos - groupStart + 1 - 4);
                                content = GetRegexSampleInternal(content, paramNames, true, ref currentParamIndex);
                                regexSample.Append(content);
                                lastAppendedLength = content.Length;
                            }
                            else
                            {
                                var placeholder = currentParamIndex >= paramNames.Length
                                                      ? defaultParamPlaceholder
                                                      : string.Format("<{0}>", paramNames[currentParamIndex++]);
                                regexSample.Append(placeholder);
                                lastAppendedLength = placeholder.Length;
                            }
                        }
                        break;
                    case '?':
                    case '*':
                        if (nesting == 0 && lastAppendedLength > 0)
                        {
                            regexSample.Remove(regexSample.Length - lastAppendedLength, lastAppendedLength);
                        }
                        break;
                    case '+':
                        //nop: we just don't add the '+'
                        break;
                    case '^':
                        if (pos > 0)
                            goto default;
                        break;
                    case '$':
                        if (pos < regex.Length - 1)
                            goto default;
                        break;
                    default:
                        if (nesting == 0)
                        {
                            regexSample.Append(ch);
                            lastAppendedLength = 1;
                        }
                        break;
                }
                pos++;
            }

            return !allowEmpty && regexSample.Length == 0 ? regex : regexSample.ToString();
        }
    }
}
