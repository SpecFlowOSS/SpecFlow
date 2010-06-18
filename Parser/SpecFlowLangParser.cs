using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Feature Parse(TextReader featureFileReader, string sourceFilePath)
        {
            var fileContent = featureFileReader.ReadToEnd();

            var language = GetLanguage(fileContent);

            I18n languageService = new I18n(language.CompatibleGherkinLanguage ?? language.Language);
            var gherkinListener = new GherkinListener(languageService);
            Feature feature = Parse(fileContent, sourceFilePath, gherkinListener, languageService);

            if (gherkinListener.Errors.Count > 0)
                throw new SpecFlowParserException(gherkinListener.Errors);

            Debug.Assert(feature != null, "If there were no errors, the feature cannot be null");
            feature.Language = language.LanguageForConversions.Name;

            return feature;
        }

        private Feature Parse(string fileContent, string sourceFilePath, GherkinListener gherkinListener, I18n languageService)
        {
            try
            {
                Lexer lexer = languageService.lexer(gherkinListener);
                lexer.scan(fileContent, sourceFilePath, 0);
                return gherkinListener.GetResult();
            }
            catch(SpecFlowParserException specFlowParserException)
            {
                foreach (var errorDetail in specFlowParserException.ErrorDetails)
                    gherkinListener.RegisterError(errorDetail);
            }
            catch (Exception ex)
            {
                gherkinListener.RegisterError(new ErrorDetail(ex));
            }
            return null;
        }

        static private readonly Regex languageRe = new Regex(@"^\s*#\s*language:\s*(?<lang>[\w-]+)\s*\n");
        private LanguageInfo GetLanguage(string fileContent)
        {
            string langName = defaultLanguage.Name;
            var langMatch = languageRe.Match(fileContent);
            if (langMatch.Success)
                langName = langMatch.Groups["lang"].Value;

            LanguageInfo languageInfo = 
                SupportedLanguageHelper.GetSupportedLanguage(langName);
            return languageInfo;
        }
    }
}
