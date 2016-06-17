using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Common;
using NUnitLite;

namespace RuntimeTests
{
    public class Program
    {
        public static int Main(string[] args)
        {
            
            return new AutoRun(typeof(Program).Assembly)
                      .Execute(args, new ExtendedTextWrapper(Console.Out), Console.In);
        }
    }
}
