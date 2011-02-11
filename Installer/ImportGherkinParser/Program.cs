using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ILMerging;
using NConsoler;

namespace ImportGherkinParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Gherkin parser import tool:");
            Console.WriteLine("  + adds signing for the assembly");
            Console.WriteLine("  + generates Language.xml from the parser");
            Console.WriteLine();

            Consolery.Run(typeof(Program), args);
            return;
        }

        static private readonly Regex parserVersionRe = new Regex(@"(?<version>[\d\.]+)");

        [Action("Imports official Gherkin parser to SpecFlow")]
        public static void ImportGherkinParser(
            [Required(Description = "Original gherkin parser, like gherkin-1.0.21.dll")] string gherkinParser,
            [Optional("specflow.snk", "key")] string keyFile,
            [Optional("Gherkin.dll", "out")] string outputFile,
            [Optional("Languages.xml", "lngout")] string languagesOutputFile
            )
        {
            string gherkinParserFullPath = Path.GetFullPath(gherkinParser);
            Version parserVersion = GetParserVersion(gherkinParserFullPath);
            if (parserVersion == null)
                return;

            string keyFullPath = Path.GetFullPath(keyFile);
            string outputFileFullPath = Path.GetFullPath(outputFile);
            string languagesOutputFileFullPath = Path.GetFullPath(languagesOutputFile);

            CreateSignedParser(gherkinParserFullPath, parserVersion, keyFullPath, outputFileFullPath);

            Assembly parserAssembly = LoadParser(outputFileFullPath);
            GenerateLanguageDescriptions(parserAssembly, languagesOutputFileFullPath);
        }

        private static Version GetParserVersion(string gherkinParserFullPath)
        {
            var match = parserVersionRe.Match(Path.GetFileNameWithoutExtension(gherkinParserFullPath));
            if (!match.Success)
            {
                Console.WriteLine("> Unable to detect parser version");
                return null;
            }
            var version = new Version(match.Groups["version"].Value);
            version = new Version(version.Major, version.Minor, version.Build, version.Revision < 0 ? 0 : version.Revision);
            return version;
        }

        private static Assembly LoadParser(string outputFileFullPath)
        {
            Console.WriteLine("Loading imported parser from {0}", outputFileFullPath);
            return Assembly.LoadFrom(outputFileFullPath);
        }

        private static void CreateSignedParser(string gherkinParserFullPath, Version version, string keyFullPath, string outputFileFullPath)
        {
            Console.WriteLine("Generating signed parser...");

            ILMerge ilMerge = new ILMerge();
            ilMerge.KeyFile = keyFullPath;
            ilMerge.Version = version;

            string simpleJsonPath = Path.Combine(Path.GetDirectoryName(gherkinParserFullPath), "com.googlecode.json-simple-json-simple.dll");
            string base46Path = Path.Combine(Path.GetDirectoryName(gherkinParserFullPath), "net.iharder-base64.dll");

            ilMerge.SetInputAssemblies(new[] { gherkinParserFullPath, simpleJsonPath, base46Path });

            ilMerge.OutputFile = outputFileFullPath;
            ilMerge.TargetKind = ILMerge.Kind.Dll;

            ilMerge.Log = true;

            ilMerge.Merge();

            Console.WriteLine();
        }

        private static readonly Dictionary<string, string> languageTranslations = 
            new Dictionary<string, string>()
            {
                {"se", "sv"},
                {"lu", "lb-LU"},
            };

        private class KeywordTranslation
        {
            public string Keyword;
            public string Translation;
        }

        class LanguageInfo
        {
            public string Code; // this is the gherkin code of the language can be either a CultureInfo or a specialized culture info
            public string CultureInfo; // this is the most specific .NET CultureInfo name that matches to code
            public string CompatibleGherkinCode; // only provided if the code was not consistent to the .NET naming (se vs. sv)
            public string DefaultSpecificCulture; // only provided is CultureInfo is a neutral language
            public string EnglishName;

            public readonly List<KeywordTranslation> KeywordTranslations = new List<KeywordTranslation>();
        }

        //ikvm__gherkin!I18nKeywords_eo.properties
        static private readonly Regex i18NResourceRe = new Regex(@"I18nKeywords_(?<lang>.+)\.properties");
        private static void GenerateLanguageDescriptions(Assembly parserAssembly, string languagesOutputFile)
        {
            List<LanguageInfo> languages = CollectSupportedLanguages(parserAssembly);
            CollectKeywordTranslations(parserAssembly, languages);

            //write languages.xml file
            XDocument document = new XDocument();
            var rootElement = new XElement(XName.Get("SpecFlowLanguages", ""));
            document.Add(rootElement);

            foreach (var language in languages)
            {
                var langElement = new XElement(XName.Get("Language", ""));
                langElement.SetAttributeValue(XName.Get("code", ""), language.Code);
                langElement.SetAttributeValue(XName.Get("cultureInfo", ""), language.CultureInfo);
                if (language.CompatibleGherkinCode != null)
                    langElement.SetAttributeValue(XName.Get("compatibleGherkinCode", ""), language.CompatibleGherkinCode);
                if (language.DefaultSpecificCulture != null)
                    langElement.SetAttributeValue(XName.Get("defaultSpecificCulture", ""), language.DefaultSpecificCulture);
                if (language.EnglishName != null)
                    langElement.SetAttributeValue(XName.Get("englishName", ""), language.EnglishName);

                foreach (var keywordTranslation in language.KeywordTranslations)
                {
                    var keywordElement = new XElement(XName.Get(keywordTranslation.Keyword, ""));
                    keywordElement.SetValue(keywordTranslation.Translation);
                    langElement.Add(keywordElement);
                }

                rootElement.Add(langElement);
            }

            document.Save(languagesOutputFile);
        }

        private static void CollectKeywordTranslations(Assembly parserAssembly, List<LanguageInfo> languages)
        {
            Type i18NType = parserAssembly.GetType("gherkin.I18n", true);
            MethodInfo keywordMethod = i18NType.GetMethod("keywords");
            if (keywordMethod == null)
                throw new InvalidOperationException("keywords method not found");

            string[] keywords = new[] { "Feature", "Background", "Scenario", "ScenarioOutline", "Examples", "Given", "When", "Then", "And", "But" };

            foreach (var language in languages)
            {
                var i18N = Activator.CreateInstance(i18NType, language.CompatibleGherkinCode ?? language.Code);

                foreach (var keyword in keywords)
                {
                    CollectKeywordTranslations(keyword, language.KeywordTranslations, i18N, keywordMethod);
                }
            }
        }

        private static void CollectKeywordTranslations(string keyword, List<KeywordTranslation> keywordTranslations, object i18N, MethodInfo keywordMethod)
        {
            string gherkinKeyword = keyword == "ScenarioOutline" ? "scenario_outline" : keyword.ToLower();
            var keywordList = keywordMethod.Invoke(i18N, new object[] { gherkinKeyword });
            object[] keywordListArray = (object[])keywordList.GetType().GetMethod("toArray", new Type[0]).Invoke(keywordList, null);
            foreach (string translation in keywordListArray)
            {
                if (!translation.Trim().StartsWith("*"))
                    keywordTranslations.Add(new KeywordTranslation { Keyword = keyword, Translation = translation.Trim() });
            }
        }

        private static List<LanguageInfo> CollectSupportedLanguages(Assembly parserAssembly)
        {
            var languages = new List<LanguageInfo>();
            var resourceNames = parserAssembly.GetManifestResourceNames();
            foreach (var resourceName in resourceNames)
            {
                var match = i18NResourceRe.Match(resourceName);
                if (!match.Success)
                    continue;

                LanguageInfo languageInfo = new LanguageInfo();
                languageInfo.Code = match.Groups["lang"].Value.Replace("_", "-");
                if (languageTranslations.ContainsKey(languageInfo.Code))
                {
                    languageInfo.CompatibleGherkinCode = languageInfo.Code;
                    languageInfo.Code = languageTranslations[languageInfo.Code];
                }

                CultureInfo cultureInfo = GetCultureInfo(languageInfo.Code);
                if (cultureInfo == null)
                    continue;

                languageInfo.CultureInfo = cultureInfo.Name;
                languageInfo.EnglishName = cultureInfo.EnglishName;
                if (cultureInfo.IsNeutralCulture)
                {
                    CultureInfo defaultSpecificCulture = GetDefaultSpecificCulture(languageInfo.Code);
                    if (defaultSpecificCulture != null)
                        languageInfo.DefaultSpecificCulture = defaultSpecificCulture.Name;
                }
                languages.Add(languageInfo);
            }
            return languages;
        }

        private static CultureInfo GetCultureInfo(string code)
        {
            try
            {
                return CultureInfo.GetCultureInfo(code);
            }
            catch (Exception)
            {
                string[] langParts = code.Split('-');
                try
                {
                    var result = CultureInfo.GetCultureInfo(langParts[0]);
                    Console.WriteLine("Possibly unusable language: {0}", code);
                    return result;
                }
                catch (Exception)
                {
                    Console.WriteLine("invlid language: {0}", code);
                    return null;
                }
            }
        }

        private static readonly Dictionary<string, string> wellKnownSpecificCultures =
            new Dictionary<string, string>()
            {
                {"en", "en-US"},
                {"no", "nb-NO"},
                {"sv", "sv-SE"},
                {"uz", "uz-Cyrl-UZ"},
                {"sr-Cyrl", "sr-Cyrl-RS"},
                {"sr-Latn", "sr-Latn-RS"},
            };

        private static CultureInfo GetDefaultSpecificCulture(string cultureName)
        {
            if (wellKnownSpecificCultures.ContainsKey(cultureName))
                return CultureInfo.GetCultureInfo(wellKnownSpecificCultures[cultureName]);

            string guessedName = cultureName + "-" + cultureName.ToUpper();
            var result = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(c => c.Name.Equals(guessedName)).FirstOrDefault();
            if (result != null)
                return result;

            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(c => c.Parent.Name.Equals(cultureName));
            result = cultures.FirstOrDefault();
            if (cultures.Count() > 1)
            {
                Console.WriteLine("Multiple possible specific cultures: {0}", string.Join(", ", cultures.Select(c => c.Name).ToArray()));
                Console.WriteLine("Default specific culture for {0} is {1}", cultureName, result);
            }
            return result;
        }
    }
}
