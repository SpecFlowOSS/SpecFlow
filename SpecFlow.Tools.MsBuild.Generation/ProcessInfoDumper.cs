using System;
using System.Diagnostics;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class ProcessInfoDumper : IProcessInfoDumper
    {
        private readonly ITaskLoggingWrapper _taskLoggingWrapper;

        public ProcessInfoDumper(ITaskLoggingWrapper taskLoggingWrapper)
        {
            _taskLoggingWrapper = taskLoggingWrapper;
        }

        public void DumpProcessInfo()
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();

                _taskLoggingWrapper.LogMessage($"process: {currentProcess.ProcessName}, pid: {currentProcess.Id}, CD: {Environment.CurrentDirectory}");

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    _taskLoggingWrapper.LogMessage($"  {assembly.FullName}");
                }
            }
            catch (Exception e)
            {
                _taskLoggingWrapper.LogMessage($"Error when dumping process info: {e}");
            }
        }
    }
}
