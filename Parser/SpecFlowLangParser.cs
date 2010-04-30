using System;
using System.Collections.Generic;     
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using gherkin;
using java.lang;
using TechTalk.SpecFlow.Parser.GherkinBuilder;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using Exception=System.Exception;

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

            //TODO: remove this hotfix when upgrading to the newer parser
            //fileContent = FixLineEndings(fileContent);

            CultureInfo language = GetLanguage(fileContent);

            //TODO: remove this hotfix when upgrading to the newer parser
            //this call has to be after the language detection, otherwise the fix removes the language comment as well
            //fileContent = FixComments(fileContent);

            var gherkinListener = new GherkinListener(language);
            Lexer lexer = new I18nLexer(gherkinListener);
            using (var reader = new StringReader(fileContent))
            {
                try
                {
                    lexer.scan(reader.ReadToEnd());
                }
                catch (Exception e)
                {
//                    gherkinListener.DisplayRecognitionError(e.Line, e.Column, e.Message);
//                    throw new SpecFlowParserException("Invalid Gherkin file!", gherkinListener.Errors);
                    throw;
                }
            }

            if (gherkinListener.Errors.Count > 0)
                throw new SpecFlowParserException("Invalid Gherkin file!", gherkinListener.Errors);

            Feature feature = gherkinListener.GetResult();

            if (feature == null)
                throw new SpecFlowParserException("Invalid Gherkin file!");

            feature.Language = language.Name;

            return feature;
        }

        static private readonly Regex commentFixRe = new Regex(@"#.*");
        private string FixComments(string fileContent)
        {
            return commentFixRe.Replace(fileContent, "");
        }

        private string FixLineEndings(string fileContent)
        {
            return fileContent.Replace("\r\n", "\n");
        }

        private IEnumerable<string> GetPossibleLanguageNames(CultureInfo language)
        {
            return GetParentChain(language).SelectMany(lang => new [] {GetGherkinLangName(lang.Name), GetGherkinLangName(lang.Name.Replace("-", ""))});
        }

        private IEnumerable<CultureInfo> GetParentChain(CultureInfo cultureInfo)
        {
            var current = cultureInfo;
            yield return current;
            while (current.Parent != current)
            {
                current = current.Parent;
                yield return current;
            }
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

        private string GetGherkinLangName(string isoLangName)
        {
            switch (isoLangName)
            {
                case "sv":
                    return "se";
                default:
                    return isoLangName;
            }
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
    }
}
