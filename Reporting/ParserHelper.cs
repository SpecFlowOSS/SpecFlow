using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Compatibility;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Reporting
{
    static class ParserHelper
    {
        public static List<Feature> GetParsedFeatures(SpecFlowProject specFlowProject)
        {
            return GetParsedFeatures(specFlowProject.FeatureFiles.Select(ff => ff.GetFullPath(specFlowProject.ProjectSettings)),
                specFlowProject.Configuration.GeneratorConfiguration.FeatureLanguage);
        }

        public static List<Feature> GetParsedFeatures(IEnumerable<string> featureFiles, CultureInfo featureLanguage)
        {
            List<Feature> parsedFeatures = new List<Feature>();
            foreach (var featureFile in featureFiles)
            {
                SpecFlowGherkinParser parser = new SpecFlowGherkinParser(featureLanguage);
                using (var reader = new StreamReader(featureFile))
                {
                    var specFlowFeature = parser.Parse(reader, featureFile);
                    var compatibleFeature = CompatibleAstConverter.ConvertToCompatibleFeature(specFlowFeature);
                    parsedFeatures.Add(compatibleFeature);
                }
            }
            return parsedFeatures;
        }
    }
}
