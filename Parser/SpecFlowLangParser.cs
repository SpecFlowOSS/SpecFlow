using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using gherkin;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Parser.GherkinBuilder;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using Exception=System.Exception;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowLangParser
    {
        private readonly GherkinLanguageServices languageServices;

        public SpecFlowLangParser(CultureInfo defaultLanguage)
        {
            this.languageServices = new GherkinLanguageServices(defaultLanguage);
        }

        public Feature Parse_(TextReader featureFileReader, string sourceFilePath)
        {
            var fileContent = featureFileReader.ReadToEnd();

            var language = languageServices.GetLanguage(fileContent);

            I18n languageService = languageServices.GetLanguageService(language);
            var gherkinListener = new GherkinListener(languageService);
            Feature feature = Parse_(fileContent, sourceFilePath, gherkinListener, languageService);

            if (gherkinListener.Errors.Count > 0)
                throw new SpecFlowParserException(gherkinListener.Errors);

            Debug.Assert(feature != null, "If there were no errors, the feature cannot be null");
            feature.Language = language.LanguageForConversions.Name;

            return feature;
        }

        private Feature Parse_(string fileContent, string sourceFilePath, GherkinListener gherkinListener, I18n languageService)
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

        public Feature Parse(TextReader featureFileReader, string sourceFilePath)
        {
            var fileContent = featureFileReader.ReadToEnd();

            var language = languageServices.GetLanguage(fileContent);

            I18n languageService = languageServices.GetLanguageService(language);
            var gherkinListener = new GherkinParserListener(sourceFilePath);
            Feature feature = Parse(fileContent, sourceFilePath, gherkinListener, languageService);

            if (gherkinListener.Errors.Count > 0)
                throw new SpecFlowParserException(gherkinListener.Errors);

            Debug.Assert(feature != null, "If there were no errors, the feature cannot be null");
            feature.Language = language.LanguageForConversions.Name;

            return feature;
        }

        private Feature Parse(string fileContent, string sourceFilePath, GherkinParserListener gherkinListener, I18n languageService)
        {
            try
            {
                GherkinScanner scanner = new GherkinScanner(languageService, fileContent);
                scanner.Scan(gherkinListener);
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
    }
}
