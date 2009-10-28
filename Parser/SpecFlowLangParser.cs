using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowLangParser
    {
        public Feature Parse(TextReader featureFileReader, string sourceFileName)
        {
            var feature = Parse(featureFileReader);
            feature.SourceFile = Path.GetFullPath(sourceFileName);
            return feature;
        }

        public Feature Parse(TextReader featureFileReader)
        {
            var fileContent = featureFileReader.ReadToEnd() + Environment.NewLine;

            var inputStream = new ANTLRReaderStream(new StringReader(fileContent));
            var lexer = new Grammar.SpecFlowLangLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Grammar.SpecFlowLangParser(tokenStream);

            var featureTree = parser.feature().Tree as CommonTree;

            if (featureTree == null || parser.ParserErrors.Count > 0 || lexer.LexerErrors.Count > 0)
            {
                throw new SpecFlowParserException("Invalid Gherkin file!", lexer.LexerErrors.Concat(parser.ParserErrors).ToArray());
            }

            var walker = new Grammar.SpecFlowLangWalker(new CommonTreeNodeStream(featureTree));

            Feature feature = walker.feature();

            if (feature == null)
                throw new SpecFlowParserException("Invalid Gherkin file!");

            return feature;
        }
    }
}
