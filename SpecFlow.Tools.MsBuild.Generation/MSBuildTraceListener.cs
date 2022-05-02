using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Tracing;

namespace SpecFlow.Tools.MsBuild.Generation;

public class MSBuildTraceListener : ITraceListener
{
    private readonly TaskLoggingHelper _taskLoggingHelper;

    public MSBuildTraceListener(TaskLoggingHelper taskLoggingHelper) 
    {
        _taskLoggingHelper = taskLoggingHelper;
    }

    public void WriteTestOutput(string message)
    {
        _taskLoggingHelper.LogMessage(MessageImportance.High, message);
    }

    public void WriteToolOutput(string message)
    {
        _taskLoggingHelper.LogMessage(MessageImportance.High, "-> " + message);
    }
}
