using NUnit.Framework;

using Rhino.Mocks;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class StepsTest
    {
        private ITestRunner mockTestRunner;
        private StepsTestableHelper steps;

        private const string Text = "text";
        private const string MultilineTextArg = "multilineTextArg";
        private readonly Table table = new Table("sausages");

        [SetUp]
        public void SetUp()
        {
            mockTestRunner = MockRepository.GenerateMock<ITestRunner>();
            ObjectContainer.CurrentTestRunner = mockTestRunner;

            steps = new StepsTestableHelper();
        }

        [Test]
        public void GivenIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.Given(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasCalled(m => m.Given(Text, MultilineTextArg, table));
        }

        [Test]
        public void WhenIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.When(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasCalled(m => m.When(Text, MultilineTextArg, table));
        }

        [Test]
        public void ThenIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.Then(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasCalled(m => m.Then(Text, MultilineTextArg, table));
        }

        [Test]
        public void ButIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.But(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasCalled(m => m.But(Text, MultilineTextArg, table));
        }

        [Test]
        public void AndIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.And(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasCalled(m => m.And(Text, MultilineTextArg, table));
        }

        public class StepsTestableHelper : Steps
        {
        }
    }
}