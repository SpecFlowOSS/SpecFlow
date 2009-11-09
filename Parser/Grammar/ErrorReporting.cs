using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Parser.Grammar
{
    partial class SpecFlowLangParser
    {
        public readonly List<string> ParserErrors = new List<string>();

        public override void DisplayRecognitionError(string[] tokenNames, Antlr.Runtime.RecognitionException e)
        {
            this.EmitErrorMessage(
                string.Format("({0},{1}): parser error: {2}", e.Line, e.CharPositionInLine, this.GetErrorMessage(e, tokenNames)));
        }

        public override void EmitErrorMessage(string msg)
        {
            Debug.WriteLine(msg);
            ParserErrors.Add(msg);
        }
    }
}
