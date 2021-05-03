using Benchmark.Feature;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using TechTalk.SpecFlow;

namespace SpecFlowBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<SingleScenarioBenchmark>();
        }
    }

    [SimpleJob(RuntimeMoniker.Net461)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [MemoryDiagnoser]
    public class SingleScenarioBenchmark
    {
        [Benchmark]
        public void Benchmark()
        {
            var currentAssembly = typeof(Program).Assembly;

            TestRunnerManager.OnTestRunStart(currentAssembly);


            var specFlowFeature1Feature = new SpecFlowFeature1Feature();
            specFlowFeature1Feature.FeatureSetup();

            specFlowFeature1Feature.TestInitialize();
            specFlowFeature1Feature.AddTwoNumbers();
            specFlowFeature1Feature.TestTearDown();

            specFlowFeature1Feature.TestInitialize();
            specFlowFeature1Feature.AddTwoNumbers();
            specFlowFeature1Feature.TestTearDown();

            specFlowFeature1Feature.TestInitialize();
            specFlowFeature1Feature.AddTwoNumbers();
            specFlowFeature1Feature.TestTearDown();

            specFlowFeature1Feature.TestInitialize();
            specFlowFeature1Feature.AddTwoNumbers();
            specFlowFeature1Feature.TestTearDown();

            specFlowFeature1Feature.FeatureTearDown();


            TestRunnerManager.OnTestRunEnd(currentAssembly);
        }
    }
}


// Features  - 10
// Scenarios - 8 Scenarios each Feature
// Scenario Outlines - 2 Outlines with 10 Examples in each feature
// Binding Classes - 20 classes
// Step Bindings - 3-5 bindings per class
// Hooks - 1 separate class + each hook once
// Step Argument Transformation - 20 in 4 classes
// Tags - one per feature, 5 scenarios have 2 per feature file