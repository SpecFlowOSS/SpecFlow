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

        public Feature Parse(TextReader featureFileReader, string sourceFilePath)
        {
            var fileContent = featureFileReader.ReadToEnd();

            var language = languageServices.GetLanguage(fileContent);

            I18n languageService = languageServices.GetLanguageService(language);
            var gherkinListener = new GherkinParserListener(sourceFilePath);

            GherkinScanner scanner = new GherkinScanner(languageService, fileContent);
            scanner.Scan(gherkinListener);

            Feature feature = gherkinListener.GetResult();

            if (gherkinListener.Errors.Count > 0)
                throw new SpecFlowParserException(gherkinListener.Errors);

            Debug.Assert(feature != null, "If there were no errors, the feature cannot be null");
            feature.Language = language.LanguageForConversions.Name;

            return feature;
        }
    }
}
