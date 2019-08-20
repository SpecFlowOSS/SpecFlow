using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{

    [Binding]
    public class ExecutionSteps
    {
        private readonly HooksDriver _hooksDriver;
        private readonly SolutionDriver _solutionDriver;
        private readonly ExecutionDriver _executionDriver;
        private Task _testExecutionTask;
        private readonly List<string> _lockedHooksMethodNames = new List<string>();

        public ExecutionSteps(HooksDriver hooksDriver, SolutionDriver solutionDriver, ExecutionDriver executionDriver)
        {
            _hooksDriver = hooksDriver;
            _solutionDriver = solutionDriver;
            _executionDriver = executionDriver;
        }

        [When(@"I execute the tests")]
        public void WhenIExecuteTheTests()
        {
            _executionDriver.ExecuteTests();
        }

        [When(@"I execute the tests twice")]
        public void WhenIExecuteTheTestsTwice()
        {
            _executionDriver.ExecuteTests();
            _executionDriver.ExecuteTests();
        }
        
        [Given(@"I start executing the tests asynchronously")]
        public void GivenIStartExecutingTheTestsAsynchronously()
        {
            _testExecutionTask = _executionDriver.ExecuteTestsAsync();
        }

        [Then(@"tests finish successfully")]
        public async Task ThenTestsFinishSuccessfully()
        {
            await _testExecutionTask;
        }

        [Given(@"hook '(.*)' log file is locked")]
        public void GivenHookLogFileIsLocked(string methodName)
        {
            _hooksDriver.AcquireHookLock();
            _lockedHooksMethodNames.Add(methodName);
        }

        [Given(@"tests are waiting for hook lock '(.*)'")]
        public async Task GivenTestsAreWaitingForHookLock(string methodName)
        {
            await _hooksDriver.WaitForIsWaitingForHookLockAsync(methodName);
            _hooksDriver.CheckIsNotHookExecuted(methodName, 1);
        }

        [When(@"hook '(.*)' file lock is released")]
        public void WhenHookFileLockIsReleased(string methodName)
        {
            _hooksDriver.ReleaseHookLock();
            _lockedHooksMethodNames.Remove(methodName);
        }

        [When(@"I execute the tests tagged with '@(.+)'")]
        public void WhenIExecuteTheTestsTaggedWithTag(string tag)
        {
            _executionDriver.ExecuteTestsWithTag(tag);
        }

        [Given(@"MSBuild is used for compiling")]
        public void GivenMSBuildIsUsedForCompiling()
        {
            _solutionDriver.CompileSolution(BuildTool.MSBuild);
        }

        [Given(@"dotnet build is used for compiling")]
        public void GivenDotnetBuildIsUsedForCompiling()
        {
            _solutionDriver.CompileSolution(BuildTool.DotnetBuild);
        }

        [Given(@"dotnet msbuild is used for compiling")]
        public void GivenDotnetMsbuildIsUsedForCompiling()
        {
            _solutionDriver.CompileSolution(BuildTool.DotnetMSBuild);
        }

        [AfterScenario]
        public void CleanupHookFileLocks()
        {
            foreach (var lockedHooksMethodName in _lockedHooksMethodNames)
            {
                _hooksDriver.ReleaseHookLock();
            }
        }
    }
}
