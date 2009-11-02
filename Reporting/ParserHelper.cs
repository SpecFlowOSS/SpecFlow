using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Configuration;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Reporting
{
    static class ParserHelper
    {
        public static List<Feature> GetParsedFeatures(SpecFlowProject specFlowProject)
        {
            return GetParsedFeatures(specFlowProject.FeatureFiles.Select(ff => ff.GetFullPath(specFlowProject)));
        }

        public static List<Feature> GetParsedFeatures(IEnumerable<string> featureFiles)
        {
            List<Feature> parsedFeatures = new List<Feature>();
            foreach (var featureFile in featureFiles)
            {
                SpecFlowLangParser parser = new SpecFlowLangParser();
                using (var reader = new StreamReader(featureFile))
                {
                    Feature feature = parser.Parse(reader, featureFile);
                    parsedFeatures.Add(feature);
                }
            }
            return parsedFeatures;
        }
    }
}
