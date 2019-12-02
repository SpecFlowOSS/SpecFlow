namespace SpecFlow.Tools.MsBuild.Generation
{
    public class MSBuildInformationProvider : IMSBuildInformationProvider
    {
        private readonly string _msbuildVersion;

        public MSBuildInformationProvider(string msbuildVersion)
        {
            _msbuildVersion = msbuildVersion;
        }

        public string GetMSBuildVersion()
        {
            return _msbuildVersion;
        }
    }
}
