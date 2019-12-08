using System.IO;
using TechTalk.SpecFlow.Generator.Project;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class SpecFlowProjectProvider : ISpecFlowProjectProvider
    {
        private readonly IMSBuildProjectReader _msbuildProjectReader;
        private readonly SpecFlowProjectInfo _specFlowProjectInfo;

        public SpecFlowProjectProvider(IMSBuildProjectReader msbuildProjectReader, SpecFlowProjectInfo specFlowProjectInfo)
        {
            _msbuildProjectReader = msbuildProjectReader;
            _specFlowProjectInfo = specFlowProjectInfo;
        }

        public SpecFlowProject GetSpecFlowProject()
        {
            var specFlowProject = _msbuildProjectReader.LoadSpecFlowProjectFromMsBuild(Path.GetFullPath(_specFlowProjectInfo.ProjectPath), _specFlowProjectInfo.RootNamespace);
            return specFlowProject;
        }
    }
}
