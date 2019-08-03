using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TechTalk.SpecFlow.Specs.MSBuild.Analyzer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var assemblyFileName = Path.GetFullPath(args.Single());

#if NETCOREAPP
            using (var resolver = new AssemblyResolver(assemblyFileName))
            {
#endif
                var options = TestFrameworkOptions.ForDiscovery();

                using (var controller = new XunitFrontController(
                    AppDomainSupport.Denied,
                    assemblyFileName))
                {
                    using (var sink = new DiscoverySink())
                    {
                        sink.TestCaseDiscoveryMessageEvent += discovery => Console.WriteLine(discovery.Message.TestCase.DisplayName);

                        controller.Find(false, sink, options);

                        await sink.Finished;
                    }
                }
#if NETCOREAPP
            }
#endif
        }
    }
}
