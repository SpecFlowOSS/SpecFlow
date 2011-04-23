//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Package;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Vs2008Integration
{
    /// <summary>
    /// This class implements IScanner interface and performs
    /// text parsing on the base of rules' table. 
    /// </summary>
    internal class RegularExpressionScanner : IScanner
    {
        public RegularExpressionScanner()
        {
            Trace.WriteLine("Create pattern table");
            patternTable = new[]
                               {
                                   //new RegularExpressionTableEntry("[A-Z]?", TokenColor.Comment),
                                   new RegularExpressionTableEntry("Scenario[ ]*:[ ]*", TokenColor.Keyword),
                                   new RegularExpressionTableEntry("Feature[ ]*:[ ]*", TokenColor.Keyword),
                                   new RegularExpressionTableEntry("When", TokenColor.Keyword),
                                   new RegularExpressionTableEntry("Given", TokenColor.Keyword),
                                   new RegularExpressionTableEntry("And", TokenColor.Keyword),
                                   new RegularExpressionTableEntry("Then", TokenColor.Keyword),
                                   new RegularExpressionTableEntry("@[a-z]*", TokenColor.Comment),
                                   new RegularExpressionTableEntry("[0-9]?", TokenColor.Number),
                                   //new RegularExpressionTableEntry("."     , TokenColor.Text)
                               };
        }

        #region Private fields
        /// <summary>
        /// Store line of text to parse
        /// </summary>
        private string sourceString;
        /// <summary>
        /// Store position where next token starts in line
        /// </summary>
        private int currentPos;

        #endregion

        #region Private static members

        /// <summary>
        /// This array contains correspondence table between regular expression patterns
        /// and color scheme of parsed text.
        /// Priority of elements decreases from first element to last element.
        /// </summary>
        private readonly RegularExpressionTableEntry[] patternTable;

        /// <summary>
        /// This method is used to compare initial string with regular expression patterns from 
        /// correspondence table
        /// </summary>
        /// <param name="source">Initial string to parse</param>
        /// <param name="charsMatched">This parameter is used to get the size of matched block</param>
        /// <param name="color">Color of matched block</param>
        private void MatchRegEx(string source, ref int charsMatched, ref TokenColor color)
        {
            foreach (RegularExpressionTableEntry tableEntry in patternTable)
            {
                bool badPattern = false;
                Regex expr = null;

                try
                {
                    // Create Regex instance using pattern from current element of associations table
                    expr = new Regex("^" + tableEntry.pattern);
                }
                catch (ArgumentException)
                {
                    badPattern = true;
                }

                if (badPattern)
                {
                    continue;
                }

                // Searching the source string for an occurrence of the regular expression pattern
                // specified in the current element of correspondence table
                Match m = expr.Match(source);
                if (m.Success && m.Length != 0)
                {
                    charsMatched = m.Length;
                    color = tableEntry.color;
                    return;
                }
            }

            // No matches found. So we return color scheme of usual text
            charsMatched = 1;
            color = TokenColor.Text;
        }

        #endregion

        #region IScanner Members

        /// <summary>
        /// This method is used to parse next language token from the current line and return information about it.
        /// </summary>
        /// <param name="tokenInfo"> The TokenInfo structure to be filled in.</param>
        /// <param name="state"> The scanner's current state value.</param>
        /// <returns>Returns true if a token was parsed from the current line and information returned;
        /// otherwise, returns false indicating no more tokens are on the current line.</returns>
        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            // If input string is empty - there is nothing to parse - so, return false
            if (sourceString.Length == 0)
            {
                return false;
            }

            TokenColor color = TokenColor.Text;
            int charsMatched = 0;
            
            // Compare input string with patterns from correspondence table
            MatchRegEx(sourceString, ref charsMatched, ref color);

            // Fill in TokenInfo structure on the basis of examination 
            if (tokenInfo != null)
            {
                tokenInfo.Color = color;
                tokenInfo.Type = TokenType.Text;
                tokenInfo.StartIndex = currentPos;
                tokenInfo.EndIndex = Math.Max(currentPos, currentPos + charsMatched - 1);
            }

            // Move current position
            currentPos += charsMatched;
            
            // Set an unprocessed part of string as a source
            sourceString = sourceString.Substring(charsMatched);

            return true;
        }

        /// <summary>
        /// This method is used to set the line to be parsed.
        /// </summary>
        /// <param name="source">The line to parse.</param>
        /// <param name="offset">The character offset in the line to start parsing from. 
        /// You have to pay attention to this value.</param>
        public void SetSource(string source, int offset)
        {
            sourceString = source;
            currentPos = offset;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Store information about patterns and colors of parsed text 
        /// </summary>
        private class RegularExpressionTableEntry
        {
            public string pattern;

            public TokenColor color;

            public RegularExpressionTableEntry(string pattern, TokenColor color)
            {
                this.pattern = pattern;
                this.color = color;
            }
        }

        #endregion
    }
}
