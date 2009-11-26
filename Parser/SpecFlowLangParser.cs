using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using TechTalk.SpecFlow.Parser.Grammar;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowLangParser
    {
        private readonly CultureInfo defaultLanguage;

        public SpecFlowLangParser(CultureInfo defaultLanguage)
        {
            this.defaultLanguage = defaultLanguage;
        }

        public Feature Parse(TextReader featureFileReader, string sourceFileName)
        {
            var feature = Parse(featureFileReader);
            feature.SourceFile = Path.GetFullPath(sourceFileName);
            return feature;
        }

        static private readonly Regex languageRe = new Regex(@"^\s*#\s*language:\s*(?<lang>[\w-]+)\s*\n");

        public Feature Parse(TextReader featureFileReader)
        {
            var fileContent = featureFileReader.ReadToEnd() + Environment.NewLine;

            CultureInfo language = GetLanguage(fileContent);

            var inputStream = new ANTLRReaderStream(new StringReader(fileContent));
            var lexer = GetLexter(language, inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Grammar.SpecFlowLangParser(tokenStream);

            var featureTree = parser.feature().Tree as CommonTree;

            if (featureTree == null || parser.ParserErrors.Count > 0 || lexer.LexerErrors.Count > 0)
            {
                throw new SpecFlowParserException("Invalid Gherkin file!", lexer.LexerErrors.Concat(parser.ParserErrors).ToArray());
            }

            var walker = new SpecFlowLangWalker(new CommonTreeNodeStream(featureTree));

            Feature feature = walker.feature();

            if (feature == null)
                throw new SpecFlowParserException("Invalid Gherkin file!");

            feature.Language = language.Name;

            return feature;
        }

        private CultureInfo GetLanguage(string fileContent)
        {
            CultureInfo language = defaultLanguage;

            var langMatch = languageRe.Match(fileContent);
            if (langMatch.Success)
            {
                string langName = langMatch.Groups["lang"].Value;
                langName = ResolveLangNameExceptions(langName);
                language = new CultureInfo(langName);
            }
            return language;
        }

        private string ResolveLangNameExceptions(string langName)
        {
            switch (langName)
            {
                case "se":
                    return "sv";
                default:
                    return langName;
            }
        }

        static readonly Dictionary<CultureInfo, Type> lexters = new Dictionary<CultureInfo, Type>
            {
                {new CultureInfo("en"), typeof(SpecFlowLangLexer_en)},
                {new CultureInfo("de"), typeof(SpecFlowLangLexer_de)},
                {new CultureInfo("fr"), typeof(SpecFlowLangLexer_fr)},
                {new CultureInfo("hu"), typeof(SpecFlowLangLexer_hu)},
                {new CultureInfo("nl"), typeof(SpecFlowLangLexer_nl)},
                {new CultureInfo("sv"), typeof(SpecFlowLangLexer_sv)},
            };

        private SpecFlowLangLexer GetLexter(CultureInfo language, ANTLRReaderStream inputStream)
        {
            Type lexterType;
            if (!lexters.TryGetValue(language, out lexterType))
            {
                CultureInfo calculatedLanguage = language;

                while (calculatedLanguage.Parent != calculatedLanguage)
                {
                    calculatedLanguage = calculatedLanguage.Parent;
                    if (lexters.TryGetValue(calculatedLanguage, out lexterType))
                        break;
                }

                if (lexterType == null)
                    throw new SpecFlowParserException(string.Format("The specified feature file language ('{0}') is not supported.", language));
            }

            return (SpecFlowLangLexer)Activator.CreateInstance(lexterType, inputStream);
        }
    }
}
