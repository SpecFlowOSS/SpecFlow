using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BoDi;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class FeatureCodeBehindGenerator : IDisposable
    {
        private readonly ITestGenerator _testGenerator;
        private readonly SpecFlowConfiguration _specFlowConfiguration;

        public FeatureCodeBehindGenerator(ITestGenerator testGenerator, SpecFlowConfiguration specFlowConfiguration)
        {
            _testGenerator = testGenerator;
            _specFlowConfiguration = specFlowConfiguration;
        }

        public TestFileGeneratorResult GenerateCodeBehindFile(string featureFile)
        {
            var featureFileInput = new FeatureFileInput(featureFile);

            var generatedFeatureFileName = Path.GetFileName(_testGenerator.GetTestFullPath(featureFileInput));

            var testGeneratorResult = _testGenerator.GenerateTestFile(featureFileInput, this.CreateGenerationSettings());

            return new TestFileGeneratorResult(testGeneratorResult, generatedFeatureFileName);
        }

        private GenerationSettings CreateGenerationSettings()
        {
            // ReSharper disable once ConvertTypeCheckPatternToNullCheck
            if (this._specFlowConfiguration.EndOfLine is string eol)
            {
                return new GenerationSettings
                {
                    EndOfLine = eol
                };
            }
            
            return new GenerationSettings();
        }

        public void Dispose()
        {
            _testGenerator?.Dispose();
        }

    }
}