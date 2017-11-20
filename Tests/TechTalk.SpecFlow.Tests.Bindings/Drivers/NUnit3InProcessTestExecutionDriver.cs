using System.Collections.Generic;

namespace TechTalk.SpecFlow.Tests.Bindings.Drivers
{
    public class NUnit3InProcessTestExecutionDriver : NUnit3TestExecutionDriverBase
    {
        public NUnit3InProcessTestExecutionDriver(InputProjectDriver inputProjectDriver, TestExecutionResult testExecutionResult)
            : base(inputProjectDriver, testExecutionResult)
        {
        }

        protected override void CallNUnit3(List<string> args)
        {
            NUnit.ConsoleRunner.Program.Main(args.ToArray());
        }
    }
}