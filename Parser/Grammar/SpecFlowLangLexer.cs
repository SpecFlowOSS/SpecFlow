using System;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime;

namespace TechTalk.SpecFlow.Parser.Grammar
{
    public abstract class SpecFlowLangLexer : Lexer
    {
        public readonly List<string> LexerErrors = new List<string>();

        protected SpecFlowLangLexer()
        {
        }

        protected SpecFlowLangLexer(ICharStream input) : base(input)
        {
        }

        protected SpecFlowLangLexer(ICharStream input, RecognizerSharedState state) : base(input, state)
        {
        }

        //public override void DisplayRecognitionError(string[] tokenNames, Antlr.Runtime.RecognitionException e)
        //{
        //    this.EmitErrorMessage(
        //        string.Format("({0},{1}): lexer error: {2}", e.Line, e.CharPositionInLine, this.GetErrorMessage(e, tokenNames)));
        //}

        //public override void EmitErrorMessage(string msg)
        //{
        //    Debug.WriteLine(msg);
        //    //LexerErrors.Add(msg);
        //}
    }
}