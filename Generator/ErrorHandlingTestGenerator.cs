using System;
using System.Diagnostics;
using System.Linq;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Generator
{
    public abstract class ErrorHandlingTestGenerator : RemotableGeneratorClass
    {
        public TestGeneratorResult GenerateTestFile(FeatureFileInput featureFileInput, GenerationSettings settings)
        {
            if (featureFileInput == null) throw new ArgumentNullException("featureFileInput");
            if (settings == null) throw new ArgumentNullException("settings");

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

        public Version DetectGeneratedTestVersion(FeatureFileInput featureFileInput)
        {
            if (featureFileInput == null) throw new ArgumentNullException("featureFileInput");

            try
            {
                return DetectGeneratedTestVersionWithExceptions(featureFileInput);
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception, "ErrorHandlingTestGenerator.DetectGeneratedTestVersion");
                return null;
            }
        }

        protected abstract TestGeneratorResult GenerateTestFileWithExceptions(FeatureFileInput featureFileInput, GenerationSettings settings);
        protected abstract Version DetectGeneratedTestVersionWithExceptions(FeatureFileInput featureFileInput);

        public virtual void Dispose()
        {
            //nop;
        }
    }
}