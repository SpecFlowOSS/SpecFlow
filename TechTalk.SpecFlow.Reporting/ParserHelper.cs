using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Reporting
{
    static class ParserHelper
    {
        public static List<SpecFlowDocument> GetParsedFeatures(SpecFlowProject specFlowProject)
        {
            return GetParsedFeatures(specFlowProject.FeatureFiles.Select(ff => ff.GetFullPath(specFlowProject.ProjectSettings)),
                specFlowProject.Configuration.SpecFlowConfiguration.FeatureLanguage);
        }

        public static List<SpecFlowDocument> GetParsedFeatures(IEnumerable<string> featureFiles, CultureInfo featureLanguage)
        {
            List<SpecFlowDocument> parsedSpecFlowDocument = new List<SpecFlowDocument>();
            foreach (var featureFile in featureFiles)
            {
                SpecFlowGherkinParser parser = new SpecFlowGherkinParser(featureLanguage);
                using (var reader = new StreamReader(featureFile))
                {
                    var specFlowFeature = parser.Parse(reader, featureFile);
                    parsedSpecFlowDocument.Add(specFlowFeature);
                }
            }
            return parsedSpecFlowDocument;
        }
    }
}
