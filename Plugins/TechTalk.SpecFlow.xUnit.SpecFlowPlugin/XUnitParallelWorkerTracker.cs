using System;
using System.Collections.Concurrent;
using System.Threading;

namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin;

public class XUnitParallelWorkerTracker
{
    public static readonly XUnitParallelWorkerTracker Instance = new();

    private readonly ConcurrentBag<string> _availableWorkers = new();
    private int _workerCount = 0;

    private XUnitParallelWorkerTracker() { }

    public string GetWorkerId()
    {
        if (!_availableWorkers.TryTake(out var workerId))
        {
            workerId = $"XW{Interlocked.Increment(ref _workerCount):D3}";
        }
        return workerId;
    }

    public void ReleaseWorker(string workerId)
    {
        _availableWorkers.Add(workerId);
    }
}