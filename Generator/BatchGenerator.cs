using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Generator
{
    public class BatchGenerator
    {
        private readonly ITraceListener traceListener;
        private readonly ITestGeneratorFactory testGeneratorFactory;

        public event Action<FeatureFileInput, TestGeneratorResult> OnSuccess;
        public event Action<FeatureFileInput, TestGeneratorResult> OnGenerated;
        public event Action<FeatureFileInput, TestGeneratorResult> OnError;

        public BatchGenerator(ITraceListener traceListener, ITestGeneratorFactory testGeneratorFactory)
        {
            this.traceListener = traceListener;
            this.testGeneratorFactory = testGeneratorFactory;
        }

        protected virtual ITestGenerator CreateGenerator(SpecFlowProject specFlowProject)
        {
            return testGeneratorFactory.CreateGenerator(specFlowProject.ProjectSettings);
        }

        public bool ProcessProject(SpecFlowProject specFlowProject, bool forceGenerate)
        {
            traceListener.WriteToolOutput("Processing project: " + specFlowProject.ProjectSettings.ProjectName);
            GenerationSettings generationSettings = GetGenerationSettings(forceGenerate);

            bool success = true;

            using (var generator = CreateGenerator(specFlowProject))
            {

                foreach (var featureFile in specFlowProject.FeatureFiles)
                {
                    var featureFileInput = CreateFeatureFileInput(featureFile, generator, specFlowProject);
                    var generationResult = GenerateTestFile(generator, featureFileInput, generationSettings);
                    if (!generationResult.Success)
                    {
                        traceListener.WriteToolOutput("{0} -> test generation failed", featureFile.ProjectRelativePath);
                        success = false;
                        if (OnError != null)
                            OnError(featureFile, generationResult);
                    }
                    else if (generationResult.IsUpToDate)
                    {
                        traceListener.WriteToolOutput("{0} -> test up-to-date", featureFile.ProjectRelativePath);
                    }
                    else
                    {
                        traceListener.WriteToolOutput("{0} -> test updated", featureFile.ProjectRelativePath);
                        if (OnGenerated != null)
                            OnGenerated(featureFile, generationResult);
                    }

                    if (generationResult.Success)
                    {
                        if (OnSuccess != null)
                            OnSuccess(featureFile, generationResult);
                    }
                }
            }

            return success;
        }

        protected virtual GenerationSettings GetGenerationSettings(bool forceGenerate)
        {
            return new GenerationSettings()
                       {
                           CheckUpToDate = !forceGenerate,
                           WriteResultToFile = true
                       };
        }

        protected virtual FeatureFileInput CreateFeatureFileInput(FeatureFileInput featureFile, ITestGenerator generator, SpecFlowProject specFlowProject)
        {
            return featureFile;
        }

        protected virtual TestGeneratorResult GenerateTestFile(ITestGenerator generator, FeatureFileInput featureFileInput, GenerationSettings generationSettings)
        {
            return generator.GenerateTestFile(featureFileInput, generationSettings);
        }
    }
}