using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Generator
{
    public class BatchGenerator
    {
        private readonly TextWriter traceWriter;
        private readonly bool verboseOutput;

        public BatchGenerator(TextWriter traceWriter, bool verboseOutput)
        {
            this.traceWriter = traceWriter;
            this.verboseOutput = verboseOutput;
        }

        public void ProcessProject(SpecFlowProject specFlowProject, bool forceGenerate)
        {
            if (verboseOutput)
                traceWriter.WriteLine("Processing project: " + specFlowProject);

            SpecFlowGenerator generator = new SpecFlowGenerator(specFlowProject);

            foreach (var featureFile in specFlowProject.FeatureFiles)
            {
                string featureFileFullPath = featureFile.GetFullPath(specFlowProject);

                string codeFileFullPath = featureFileFullPath + ".cs";

                bool needsProcessing = true;

                if (!forceGenerate && IsUpToDate(generator, featureFileFullPath, codeFileFullPath))
                {
                    needsProcessing = false;
                }

                if (verboseOutput || needsProcessing)
                {
                    traceWriter.WriteLine("Processing file: {0}", featureFileFullPath);
                    if (!needsProcessing)
                        traceWriter.WriteLine("  up-to-date");
                }

                if (needsProcessing)
                {
                    GenerateFile(generator, featureFile, codeFileFullPath);
                }
            }
        }

        protected virtual void GenerateFile(SpecFlowGenerator generator, SpecFlowFeatureFile featureFile, string codeFileFullPath)
        {
            using (var writer = GetWriter(codeFileFullPath))
            {
                generator.GenerateCSharpTestFile(featureFile, writer);
            }
        }

        protected virtual StreamWriter GetWriter(string codeFileFullPath)
        {
            return new StreamWriter(codeFileFullPath, false, Encoding.UTF8);
        }

        private static bool IsUpToDate(SpecFlowGenerator generator, string featureFileFullPath, string codeFileFullPath)
        {
            // check existance of the target file
            if (!File.Exists(codeFileFullPath))
                return false;

            // check modification time of the target file
            try
            {
                var featureFileModificationTime = File.GetLastWriteTime(featureFileFullPath);
                var codeFileModificationTime = File.GetLastWriteTime(codeFileFullPath);

                if (featureFileModificationTime > codeFileModificationTime)
                    return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            // check tools version
            var codeFileVersion = generator.GetGeneratedFileSpecFlowVersion(codeFileFullPath);
            if (codeFileVersion == null || codeFileVersion != generator.GetCurrentSpecFlowVersion())
                return false;

            return true;
        }
    }
}