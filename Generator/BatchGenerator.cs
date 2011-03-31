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

        public BatchGenerator(ITraceListener traceListener, ITestGeneratorFactory testGeneratorFactory)
        {
            this.traceListener = traceListener;
            this.testGeneratorFactory = testGeneratorFactory;
        }

        protected virtual ITestGenerator CreateGenerator(SpecFlowProject specFlowProject)
        {
            return testGeneratorFactory.CreateGenerator(specFlowProject.ConfigurationHolder, specFlowProject.ProjectSettings);
        }

        public void ProcessProject(SpecFlowProject specFlowProject, bool forceGenerate)
        {
            traceListener.WriteToolOutput("Processing project: " + specFlowProject);
            GenerationSettings generationSettings = new GenerationSettings()
                                                        {
                                                            CheckUpToDate = !forceGenerate,
                                                            WriteResultToFile = true
                                                        };

            using (var generator = CreateGenerator(specFlowProject))
            {

                foreach (var featureFile in specFlowProject.FeatureFiles)
                {
                    string featureFileFullPath = featureFile.GetFullPath(specFlowProject);
                    traceListener.WriteToolOutput("Processing file: {0}", featureFileFullPath);

                    var featureFileInput = CreateFeatureFileInput(featureFile, generator, specFlowProject);
                    var generationResult = GenerateTestFile(generator, featureFileInput, generationSettings);
                    if (!generationResult.Success)
                    {
                        traceListener.WriteToolOutput("  generation failed");
                    }
                    else if (generationResult.IsUpToDate)
                    {
                        traceListener.WriteToolOutput("  up-to-date");
                    }
                }
            }
        }

        protected virtual FeatureFileInput CreateFeatureFileInput(SpecFlowFeatureFile featureFile, ITestGenerator generator, SpecFlowProject specFlowProject)
        {
            return featureFile.ToFeatureFileInput();
        }

        protected virtual TestGeneratorResult GenerateTestFile(ITestGenerator generator, FeatureFileInput featureFileInput, GenerationSettings generationSettings)
        {
            return generator.GenerateTestFile(featureFileInput, generationSettings);
        }
    }
}