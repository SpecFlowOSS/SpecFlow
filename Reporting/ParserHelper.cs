using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Reporting
{
    static class ParserHelper
    {
        public static List<Feature> GetParsedFeatures(SpecFlowProject specFlowProject)
        {
            List<Feature> parsedFeatures = new List<Feature>();
            foreach (var featureFile in specFlowProject.FeatureFiles)
            {
                SpecFlowLangParser parser = new SpecFlowLangParser();
                using (var reader = new StreamReader(featureFile.GetFullPath(specFlowProject)))
                {
                    Feature feature = parser.Parse(reader);
                    parsedFeatures.Add(feature);
                }
            }
            return parsedFeatures;
        }
    }
}
