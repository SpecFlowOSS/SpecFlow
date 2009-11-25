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
            var node = ((ITree)input.LT(1));
            return new FilePosition(node.Line, node.CharPositionInLine - node.Parent.Text.Length);
        }

        protected FilePosition ParseFilePosition(string text)
        {
            var parts = text.Split(':');
            return new FilePosition(int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }
}
