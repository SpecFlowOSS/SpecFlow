using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.Grammar
{
    public abstract class SpecFlowLangWalkerBase : TreeParser
    {
        protected SpecFlowLangWalkerBase(ITreeNodeStream input) : base(input)
        {
        }

        protected SpecFlowLangWalkerBase(ITreeNodeStream input, RecognizerSharedState state) : base(input, state)
        {
        }

        protected FilePosition GetFilePosition()
        {
            return new FilePosition(((ITree)input.LT(1)).Line, ((ITree)input.LT(1)).CharPositionInLine);
        }
    }
}
