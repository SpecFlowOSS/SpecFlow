using BoDi;
using Moq;
using Xunit;


using TechTalk.SpecFlow.Infrastructure;
#pragma warning disable 618

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
        public void GivenIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.Given(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.Given(Text, MultilineTextArg, table, null));
        }

        [Fact]
        public void WhenIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.When(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.When(Text, MultilineTextArg, table, null));
        }

        [Fact]
        public void ThenIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.Then(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.Then(Text, MultilineTextArg, table, null));
        }

        [Fact]
        public void ButIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.But(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.But(Text, MultilineTextArg, table, null));
        }

        [Fact]
        public void AndIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.And(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.And(Text, MultilineTextArg, table, null));
        }

        public class StepsTestableHelper : Steps
        {
        }
    }
}