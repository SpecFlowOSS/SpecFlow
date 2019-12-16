namespace SpecFlow.Tools.MsBuild.Generation
{
    public class AssemblyResolveLoggerFactory : IAssemblyResolveLoggerFactory
    {
        private readonly ITaskLoggingWrapper _taskLoggingWrapper;

        public AssemblyResolveLoggerFactory(ITaskLoggingWrapper taskLoggingWrapper)
        {
            _taskLoggingWrapper = taskLoggingWrapper;
        }

        public IAssemblyResolveLogger Build()
        {
            return new AssemblyResolveLogger(_taskLoggingWrapper);
        }
    }
}
