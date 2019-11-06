using System.Collections.Generic;
using BoDi;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public interface IFeatureFileCodeBehindGenerator
    {
        IEnumerable<string> GenerateFilesForProject(List<string> featureFiles, string projectFolder, string outputPath);

    }
}