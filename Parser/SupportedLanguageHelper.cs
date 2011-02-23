using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TechTalk.SpecFlow.Parser
{
    internal class LanguageInfo
    {
        public string Language;
        public string CompatibleGherkinLanguage;
        public CultureInfo CultureInfo;
        public CultureInfo LanguageForConversions;

        public override bool Equals(object obj)
        {
            LanguageInfo other = obj as LanguageInfo;

            return other != null && other.Language.Equals(Language);
        }

        public override int GetHashCode()
        {
            return Language.GetHashCode();
        }
    }

    internal static class SupportedLanguageHelper
    {
        static readonly XDocument languageDoc;

        static SupportedLanguageHelper()
        {
            var docStream = typeof(SupportedLanguageHelper).Assembly.GetManifestResourceStream("TechTalk.SpecFlow.Parser.Languages.xml");
            if (docStream == null)
                throw new InvalidOperationException("Language resource not found.");
            
            using (var reader = new StreamReader(docStream))
            {
                languageDoc = XDocument.Load(reader);
            }
        }

        public static LanguageInfo GetSupportedLanguage(string languageName)
        {
            var langElm = GetBestFitLanguageElement(languageDoc, languageName);
            return LoadLangInfo(langElm, languageName);
        }

        private static LanguageInfo LoadLangInfo(XElement langElm, string requestedLanguageName)
        {
            var langInfo = new LanguageInfo();
            langInfo.Language = langElm.Attribute(XName.Get("code", "")).Value;
            langInfo.CultureInfo = CultureInfo.GetCultureInfo(langElm.Attribute(XName.Get("cultureInfo", "")).Value);

            var gherkinCodeAttr = langElm.Attribute(XName.Get("compatibleGherkinCode", ""));
            if (gherkinCodeAttr != null)
                langInfo.CompatibleGherkinLanguage = gherkinCodeAttr.Value;

            if (langInfo.CultureInfo.IsNeutralCulture)
            {
                langInfo.LanguageForConversions = GetLanguageForConversions(langElm, requestedLanguageName);
            }
            else
            {
                langInfo.LanguageForConversions = langInfo.CultureInfo;
            }
            return langInfo;
        }

        private static CultureInfo GetLanguageForConversions(XElement langElm, string requestedLanguageName)
        {
            try
            {
                var requestedCulture = CultureInfo.GetCultureInfo(requestedLanguageName);
                if (!requestedCulture.IsNeutralCulture)
                    return requestedCulture;
            }
            catch (Exception)
            {
                //nop
            }

            var defaultSpecificCultureAttr = langElm.Attribute(XName.Get("defaultSpecificCulture", ""));
            if (defaultSpecificCultureAttr == null)
                return CultureInfo.GetCultureInfo("en-US");

            return CultureInfo.GetCultureInfo(defaultSpecificCultureAttr.Value);
        }

        private static XElement GetBestFitLanguageElement(XDocument languageDoc, string langName)
        {
            var langElm = GetLanguageElement(languageDoc, langName);
            if (langElm != null)
                return langElm;

            int lastDashIndex = langName.LastIndexOf('-');
            if (lastDashIndex <= 0)
                throw new SpecFlowParserException(new ErrorDetail
                                                      {
                                                          Line = 1,
                                                          Column = 1,
                                                          Message = string.Format("The specified feature file language ('{0}') is not supported.", langName)
                                                      });

            return GetBestFitLanguageElement(languageDoc, langName.Substring(0, lastDashIndex));
        }

        private static XElement GetLanguageElement(XDocument languageDoc, string langName)
        {
            Debug.Assert(languageDoc.Root != null);
            return languageDoc.Root.Elements("Language").FirstOrDefault(l => IsForLanguage(l, langName));
        }

        private static bool IsForLanguage(XElement langElm, string langName)
        {
            if (langElm.Attribute("code").Value == langName)
                return true;
            var compAttr = langElm.Attribute("compatibleGherkinCode");
            return compAttr != null && compAttr.Value == langName;
        }
    }
}