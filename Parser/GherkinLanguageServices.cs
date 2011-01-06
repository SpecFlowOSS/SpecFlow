using System;
using System.Globalization;
using System.Text.RegularExpressions;
using gherkin;

namespace TechTalk.SpecFlow.Parser
{
    public class GherkinLanguageServices
    {
        private readonly CultureInfo defaultLanguage;

        public GherkinLanguageServices(CultureInfo defaultLanguage)
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

        internal I18n GetLanguageService(LanguageInfo language)
        {
            return new I18n(language.CompatibleGherkinLanguage ?? language.Language);
        }

        public I18n GetLanguageService(string fileContent)
        {
            var language = GetLanguage(fileContent);
            return GetLanguageService(language);
        }

        public I18n GetLanguageService(Func<int, string> lineProvider)
        {
            var language = GetLanguage(lineProvider);
            return GetLanguageService(language);
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
    }
}