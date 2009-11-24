using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Configuration;

namespace TechTalk.SpecFlow.GeneratorTools.BatchTestGeneration
{
    public static class BatchTestGenerator
    {
        public static void ProcessProject(SpecFlowProject specFlowProject, bool forceGenerate, bool verbose)
        {
            if (verbose)
                Console.WriteLine("Processing project: " + specFlowProject);

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

                if (verbose || needsProcessing)
                {
                    Console.WriteLine("Processing file: {0}", featureFileFullPath);
                    if (!needsProcessing)
                        Console.WriteLine("  up-to-date");
                }

                if (needsProcessing)
                {
                    using (var writer = new StreamWriter(codeFileFullPath, false, Encoding.UTF8))
                    {
                        generator.GenerateCSharpTestFile(featureFile, writer);
                    }
                }
            }
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