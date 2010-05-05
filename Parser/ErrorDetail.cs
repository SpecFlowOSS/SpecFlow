using System;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Parser
{
    [Serializable]
    public class ErrorDetail
    {
        public string Message { get; set; }
        public int? Column { get; set; } // beginning of the line: 1
        public int? Line { get; set; } // beginning of the file: 1

        public int ForcedLine
        {
            get { return Line ?? 1; }
        }

        public int ForcedColumn
        {
            get { return Column ?? 1; }
        }

        public ErrorDetail()
        {
        }

        //Message	"Lexing error on line 13: 'Thenx the result should be 120 on the screen\r\n\r\n\n%_FEATURE_END_%'"	string
        private static readonly Regex lexingErrorRe = new Regex(@"^Lexing error on line (?<line>\d+):");
        private static readonly Regex firstLineRe = new Regex(@"^(?<firstline>[^\n\r]*)");

        public ErrorDetail(Exception ex)
        {
            var match = lexingErrorRe.Match(ex.Message);
            if (match.Success)
            {
                Line = int.Parse(match.Groups["line"].Value);
                Message = firstLineRe.Match(ex.Message).Groups["firstline"].Value;
            }
            else
            {
                Message = ex.Message.Replace("\r", "").Replace("\n", "\r\n");
            }
        }
    }
}