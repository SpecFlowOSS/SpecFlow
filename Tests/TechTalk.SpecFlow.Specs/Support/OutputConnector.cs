using TechTalk.SpecFlow.TestProjectGenerator;

namespace TechTalk.SpecFlow.Specs.Support
{
    class OutputConnector : IOutputWriter
    {
        private readonly ISpecFlowOutputHelper _specFlowOutputHelper;

        public OutputConnector(ISpecFlowOutputHelper specFlowOutputHelper)
        {
            _specFlowOutputHelper = specFlowOutputHelper;
        }

        public void WriteLine(string message)
        {
            _specFlowOutputHelper.WriteLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            _specFlowOutputHelper.WriteLine(format, args);
        }
    }
}
