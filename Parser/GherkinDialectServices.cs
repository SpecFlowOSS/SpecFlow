using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using gherkin;

namespace TechTalk.SpecFlow.Parser
{
    public class GherkinDialectServices
    {
        private readonly CultureInfo defaultLanguage;

        public CultureInfo DefaultLanguage
        {
            get { return defaultLanguage; }
        }

        public GherkinDialectServices(CultureInfo defaultLanguage)
        {
            this.defaultLanguage = defaultLanguage;
        }

        static private readonly Regex languageRe = new Regex(@"^\s*#\s*language:\s*(?<lang>[\w-]+)\s*\n");
        static private readonly Regex languageLineRe = new Regex(@"^\s*#\s*language:\s*(?<lang>[\w-]+)\s*$");
        internal LanguageInfo GetLanguage(string fileContent)
        {
            string langName = defaultLanguage.Name;
            var langMatch = languageRe.Match(fileContent);
            if (langMatch.Success)
                langName = langMatch.Groups["lang"].Value;

            LanguageInfo languageInfo =
                SupportedLanguageHelper.GetSupportedLanguage(langName);
            return languageInfo;
        }

        public GherkinDialect GetDefaultDialect()
        {
            return GetGherkinDialect(SupportedLanguageHelper.GetSupportedLanguage(defaultLanguage.Name));
        }

        internal GherkinDialect GetGherkinDialect(LanguageInfo language)
        {
            return new GherkinDialect(language, 
                new I18n(language.CompatibleGherkinLanguage ?? language.Language));
        }

        public GherkinDialect GetGherkinDialect(Feature feature)
        {
            string langName =  feature.Language ?? defaultLanguage.Name;
            var language = SupportedLanguageHelper.GetSupportedLanguage(langName);
            return GetGherkinDialect(language);
        }

        public GherkinDialect GetGherkinDialect(string fileContent)
        {
            var language = GetLanguage(fileContent);
            return GetGherkinDialect(language);
        }

        public GherkinDialect GetGherkinDialect(Func<int, string> lineProvider)
        {
            var language = GetLanguage(lineProvider);
            return GetGherkinDialect(language);
        }

        internal LanguageInfo GetLanguage(Func<int, string> lineProvider)
        {
            string langName = defaultLanguage.Name;
            int lineNo = 0;
            string line;
            while ((line = lineProvider(lineNo++)) != null)
            {
                var langMatch = languageLineRe.Match(line);
                if (langMatch.Success)
                {
                    langName = langMatch.Groups["lang"].Value;
                    break;
                }

                if (line.Trim().Length != 0)
                    break;
            }

            return SupportedLanguageHelper.GetSupportedLanguage(langName);
        }

        static public bool IsLanguageLine(string line)
        {
            return languageLineRe.Match(line).Success;
        }
    }
}