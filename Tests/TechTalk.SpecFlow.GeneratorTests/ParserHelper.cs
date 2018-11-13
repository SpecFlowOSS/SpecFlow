using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gherkin.Ast;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.GeneratorTests
{
    class ParserHelper
    {
        public static SpecFlowDocument CreateAnyDocument(string[] tags = null)
        {
            var specFlowFeature = new SpecFlowFeature(GetTags(tags), null, null, null, null, null, null);
            return new SpecFlowDocument(specFlowFeature, new Comment[0], null);
        }

        public static Tag[] GetTags(params string[] tags)
        {
            return tags == null ? new Tag[0] : tags.Select(t => new Tag(null, t)).ToArray();
        }

        public static SpecFlowDocument CreateDocument(string[] tags = null, string[] scenarioTags = null)
        {
            tags = tags ?? new string[0];

            var scenario1 = new Scenario(GetTags(scenarioTags), null, "Scenario", "scenario1 title", "", new Step[0], new Examples[0]);

            var specFlowFeature = new SpecFlowFeature(GetTags(tags), null, "en", "feature", "title", "desc", new StepsContainer[] {scenario1});
            return new SpecFlowDocument(specFlowFeature, new Comment[0], null);
        }
        public static SpecFlowDocument CreateDocumentWithScenarioOutline(string[] tags = null, string[] scenarioOutlineTags = null, string[] examplesTags = null)
        {
            tags = tags ?? new string[0];

            var scenario1 = new ScenarioOutline(GetTags(scenarioOutlineTags), null, "Scenario Outline", "scenario outline1 title", "", new Step[0], new []
            {
                new Examples(GetTags(examplesTags), null, "Examples", "examples name", "", new Gherkin.Ast.TableRow(null, new []{ new TableCell(null, "col1"), }), new Gherkin.Ast.TableRow[0])
            });

            var specFlowFeature = new SpecFlowFeature(GetTags(tags), null, "en", "feature", "title", "desc", new StepsContainer[] {scenario1});
            return new SpecFlowDocument(specFlowFeature, new Comment[0], null);
        }
    }
}
