using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class AsyncExceptionSteps
    {
        [Given(@"some basic setup")]
        public void GivenSomeBasicSetup()
        {
            Console.WriteLine("the given");
        }

        [When(@"some basic async action")]
        public async Task WhenSomeBasicAsyncAction()
        {
            List<string> list = null;
            list.Where(x => x.Contains("whatever"));

            await Task.Delay(0);
        }

        [Then(@"some basic result")]
        public void ThenSomeBasicResult()
        {
            Console.WriteLine("the then");
        }
    }
}
