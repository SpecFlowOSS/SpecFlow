using System.Threading.Tasks;
using BoDi;
using Moq;
using Xunit;


using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests
{
    
    public class StepsTest
    {
        private Mock<ITestRunner> mockTestRunner;
        private StepsTestableHelper steps;

        private const string Text = "text";
        private const string MultilineTextArg = "multilineTextArg";
        private readonly Table table = new Table("sausages");

        public StepsTest()
        {
            mockTestRunner = new Mock<ITestRunner>();
            var container = new ObjectContainer();
            container.RegisterInstanceAs(mockTestRunner.Object);
            steps = new StepsTestableHelper();
            ((IContainerDependentObject)steps).SetObjectContainer(container);
        }

        [Fact]
        public async Task GivenIsDelegatedToObjectContainerCurrentTestRunner()
        {
            await steps.GivenAsync(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.GivenAsync(Text, MultilineTextArg, table, null));
        }

        [Fact]
        public async Task WhenIsDelegatedToObjectContainerCurrentTestRunner()
        {
            await steps.WhenAsync(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.WhenAsync(Text, MultilineTextArg, table, null));
        }

        [Fact]
        public async Task ThenIsDelegatedToObjectContainerCurrentTestRunner()
        {
            await steps.ThenAsync(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.ThenAsync(Text, MultilineTextArg, table, null));
        }

        [Fact]
        public async Task ButIsDelegatedToObjectContainerCurrentTestRunner()
        {
            await steps.ButAsync(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.ButAsync(Text, MultilineTextArg, table, null));
        }

        [Fact]
        public async Task AndIsDelegatedToObjectContainerCurrentTestRunner()
        {
            await steps.AndAsync(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.AndAsync(Text, MultilineTextArg, table, null));
        }

        public class StepsTestableHelper : Steps
        {
        }
    }
}