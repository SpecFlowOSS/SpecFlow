using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class ProjectCodeBehindGenerator : IProjectCodeBehindGenerator
    {
        private readonly IFeatureFileCodeBehindGenerator _featureFileCodeBehindGenerator;
        private readonly SpecFlowProjectInfo _specFlowProjectInfo;

        public ProjectCodeBehindGenerator(IFeatureFileCodeBehindGenerator featureFileCodeBehindGenerator, SpecFlowProjectInfo specFlowProjectInfo)
        {
            _featureFileCodeBehindGenerator = featureFileCodeBehindGenerator;
            _specFlowProjectInfo = specFlowProjectInfo;
        }

        public IReadOnlyCollection<ITaskItem> GenerateCodeBehindFilesForProject()
        {
            var generatedFiles = _featureFileCodeBehindGenerator.GenerateFilesForProject(
                _specFlowProjectInfo.FeatureFiles,
                _specFlowProjectInfo.ProjectFolder,
                _specFlowProjectInfo.OutputPath);

            return generatedFiles.Select(file => new TaskItem { ItemSpec = file })
                                 .Cast<ITaskItem>()
                                 .ToArray();
        }
    }
}
