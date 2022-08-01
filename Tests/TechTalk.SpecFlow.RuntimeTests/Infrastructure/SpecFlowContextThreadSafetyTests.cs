using System.Collections.Generic;
using System.Threading.Tasks;
using BoDi;
using Moq;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    /// <summary>
    ///     Test class which verifies that the <see cref="ScenarioContext"/> and <see cref="FeatureContext"/> Dictionary is thread-safe.
    /// </summary>
    public class SpecFlowContextThreadSafetyTests : StepExecutionTestsBase
    {
        private ContextManager CreateContextManager(IObjectContainer testThreadContainer)
        {
            return new ContextManager(new Mock<ITestTracer>().Object, testThreadContainer, ContainerBuilderStub);
        }

        [Fact]
        public void Test_Scenario_Context_Thread_Safety()
        {
            var containerBuilder = new RuntimeTestsContainerBuilder();
            var testThreadContainer = containerBuilder.CreateTestThreadContainer(containerBuilder.CreateGlobalContainer(typeof(SpecFlowContextThreadSafetyTests).Assembly));
            var contextManager = CreateContextManager(testThreadContainer);
            contextManager.InitializeFeatureContext(new FeatureInfo(FeatureLanguage, "", "test feature", null));
            contextManager.InitializeScenarioContext(new ScenarioInfo("test scenario", "test_description", null, null));

            // setup a list containing unique numbers.
            var taskNumbers = new List<int>();
            while (taskNumbers.Count < 100) taskNumbers.Add(taskNumbers.Count + 1);

            // run a task for each number in the list which sets the ScenarioContext item to this unique number in parallel
            Parallel.ForEach(
                taskNumbers, (i) =>
                {
                    contextManager.ScenarioContext[$"key_{i}"] = i;
                });
            
            // Verify that all of the ScenarioContext items has their expected value. 
            // This fails with different kind of errors when SpecFlowContext inherits from Dictionary in stead of ConcurrentDictionary.
            Assert.All(taskNumbers, (i) =>
            {
                var contextItem = (int) contextManager.ScenarioContext[$"key_{i}"]; 
                Assert.Equal(contextItem , i);
            });
        }
        
        [Fact]
        public void Test_Feature_Context_Thread_Safety()
        {
            var containerBuilder = new RuntimeTestsContainerBuilder();
            var testThreadContainer = containerBuilder.CreateTestThreadContainer(containerBuilder.CreateGlobalContainer(typeof(SpecFlowContextThreadSafetyTests).Assembly));
            var contextManager = CreateContextManager(testThreadContainer);
            contextManager.InitializeFeatureContext(new FeatureInfo(FeatureLanguage, "", "test feature", null));
            contextManager.InitializeScenarioContext(new ScenarioInfo("test scenario", "test_description", null, null));

            // setup a list containing unique numbers.
            var taskNumbers = new List<int>();
            while (taskNumbers.Count < 100) taskNumbers.Add(taskNumbers.Count + 1);

            // run a task for each number in the list which sets the FeatureContext item to this unique number in parallel
            Parallel.ForEach(
                taskNumbers, (i) =>
                {
                    contextManager.FeatureContext[$"key_{i}"] = i;
                });
            
            // Verify that all of the FeatureContext items has their expected value. 
            // This fails with different kind of errors when SpecFlowContext inherits from Dictionary in stead of ConcurrentDictionary.
            Assert.All(taskNumbers, (i) =>
            {
                var contextItem = (int) contextManager.FeatureContext[$"key_{i}"]; 
                Assert.Equal(contextItem , i);
            });
        }
    }
}
