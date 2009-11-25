using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Parser.SyntaxElements;

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

        protected string GetFilePosition()
        {
            var node = input.LT(1);
            return string.Format("{0}:{1}", node.Line, node.CharPositionInLine + 1);
        }

        public void Hehe()
        {
            Debug.WriteLine("hehe");
        }
    }
}
