using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Compatibility;


namespace TechTalk.SpecFlow.Tracing
{
    internal static class LanguageHelper
    {
        struct KeywordSet
        {
            public string DefaultKeyword
            {
                get { return Keywords.First(); }
            }
            public readonly string[] Keywords;

            public KeywordSet(string[] keywords)
            {
                this.Keywords = keywords;
            }
        }

        class KeywordTranslation : Dictionary<StepDefinitionKeyword, KeywordSet>
        {
            public CultureInfo DefaultSpecificCulture { get; set; }
        }

        private static readonly Dictionary<CultureInfo, KeywordTranslation> translationCache = new Dictionary<CultureInfo, KeywordTranslation>();

        public static string GetDefaultKeyword(CultureInfo language, StepDefinitionType stepDefinitionType)
        {
            return GetDefaultKeyword(language, stepDefinitionType.ToStepDefinitionKeyword());
        }

        public static string[] GetKeywords(CultureInfo language, StepDefinitionType stepDefinitionType)
        {
            return GetKeywords(language, stepDefinitionType.ToStepDefinitionKeyword());
        }

        public static string[] GetKeywords(CultureInfo language, StepDefinitionKeyword keyword)
        {
            KeywordTranslation translation = GetTranslation(language);
            return translation[keyword].Keywords;
        }

        public static string GetDefaultKeyword(CultureInfo language, StepDefinitionKeyword keyword)
        {
            KeywordTranslation translation = GetTranslation(language);
            return translation[keyword].DefaultKeyword;
        }

        private static KeywordTranslation GetTranslation(CultureInfo language)
        {
            KeywordTranslation translation;
            if (!translationCache.TryGetValue(language, out translation))
            {
                translation = LoadTranslation(language);
            }
            return translation;
        }

        static public CultureInfo GetSpecificCultureInfo(CultureInfo language)
        {
            //HACK: we need to have a better solution
            if (!language.IsNeutralCulture)
                return language;

            KeywordTranslation translation = GetTranslation(language);
            return translation.DefaultSpecificCulture;
        }


        private static KeywordTranslation LoadTranslation(CultureInfo language)
        {
            var assembly = typeof(LanguageHelper).Assembly;
            var docStream = assembly.GetManifestResourceStream("TechTalk.SpecFlow.Languages.xml");
            if (docStream == null)
                throw new InvalidOperationException("Language resource not found.");

            XDocument languageDoc;
            using (var reader = new StreamReader(docStream))
            {
                languageDoc = XDocument.Load(reader);
            }

            XElement langElm = GetBestFitLanguageElement(languageDoc, language);

            KeywordTranslation result = new KeywordTranslation();

            if (language.IsNeutralCulture)
            {
                var defaultSpecificCultureAttr = langElm.Attribute(XName.Get("defaultSpecificCulture", ""));
                if (defaultSpecificCultureAttr == null)
                    result.DefaultSpecificCulture = CultureInfoHelper.GetCultureInfo("en-US");
                else
                    result.DefaultSpecificCulture = CultureInfoHelper.GetCultureInfo(defaultSpecificCultureAttr.Value);
            }
            else
            {
                result.DefaultSpecificCulture = language;
            }

            foreach (StepDefinitionKeyword keyword in EnumHelper.GetValues(typeof (StepDefinitionKeyword)))
            {
                var keywordList = langElm.Elements(keyword.ToString()).Select(keywordElm => keywordElm.Value).ToArray();
                result[keyword] = new KeywordSet(keywordList);
            }

            return result;
        }

        private static XElement GetBestFitLanguageElement(XDocument languageDoc, CultureInfo language)
        {
            var langElm = GetLanguageElement(languageDoc, language);
            if (langElm == null)
            {
                CultureInfo calculatedLanguage = language;

                while (calculatedLanguage.Parent != calculatedLanguage)
                {
                    calculatedLanguage = calculatedLanguage.Parent;

                    langElm = GetLanguageElement(languageDoc, calculatedLanguage);
                    if (langElm != null)
                        break;
                }

                if (langElm == null)
                    throw new SpecFlowException(string.Format("The specified feature file language ('{0}') is not supported.", language));
            }
            return langElm;
        }

        private static XElement GetLanguageElement(XDocument languageDoc, CultureInfo language)
        {
            Debug.Assert(languageDoc.Root != null);
            return languageDoc.Root.Elements("Language").SingleOrDefault(l => l.Attribute("code").Value == language.Name);
        }
    }
}
