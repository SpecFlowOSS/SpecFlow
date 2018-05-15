using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class SpecFlowOutputHelper : ISpecFlowOutputHelper
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
    }
}
