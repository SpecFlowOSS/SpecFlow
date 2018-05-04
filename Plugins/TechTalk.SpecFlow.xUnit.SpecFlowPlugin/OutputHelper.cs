using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Infrastructure;
using Xunit.Abstractions;

namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin
{
    class OutputHelper : ISpecFlowOutputHelper
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public OutputHelper(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void WriteLine(string message)
        {
            _testOutputHelper.WriteLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            _testOutputHelper.WriteLine(format, args);
        }
    }
}
