using System;
using System.Linq;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Generator
{
    public abstract class ErrorHandlingTestGenerator : ITestGenerator
    {
        public TestGeneratorResult GenerateTestFile(FeatureFileInput featureFileInput, GenerationSettings settings)
        {
            try
            {
                return GenerateTestFileWithExceptions(featureFileInput, settings);
            }
            catch (SpecFlowParserException parserException)
            {
                if (parserException.ErrorDetails == null || parserException.ErrorDetails.Count == 0)
                    return new TestGeneratorResult(new TestGenerationError(parserException));

                return new TestGeneratorResult(parserException.ErrorDetails.Select(
                    ed => new TestGenerationError(ed.ForcedLine - 1, ed.ForcedColumn - 1, ed.Message)));
            }
            catch (Exception exception)
            {
                return new TestGeneratorResult(new TestGenerationError(exception));
            }
        }

        protected abstract TestGeneratorResult GenerateTestFileWithExceptions(FeatureFileInput featureFileInput, GenerationSettings settings);
    }
}