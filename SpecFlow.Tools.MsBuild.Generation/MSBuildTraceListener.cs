using TechTalk.SpecFlow.Tracing;

namespace SpecFlow.Tools.MsBuild.Generation;

public class MSBuildTraceListener : ITraceListener
{
    private readonly ITaskLoggingWrapper _taskLoggingWrapper;

    public MSBuildTraceListener(ITaskLoggingWrapper taskLoggingWrapper) 
    {
        _taskLoggingWrapper = taskLoggingWrapper;
    }

    public void WriteTestOutput(string message)
    {
        _taskLoggingWrapper.LogMessage(message);
    }

    public void WriteToolOutput(string message)
    {
        _taskLoggingWrapper.LogMessage("-> " + message);
    }
}
