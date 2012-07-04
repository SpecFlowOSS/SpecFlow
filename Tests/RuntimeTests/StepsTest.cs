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
            steps = new StepsTestableHelper(mockTestRunner);
        }

        [Test]
        public void GivenIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.Given(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasCalled(m => m.Given(Text, MultilineTextArg, table, null));
        }

        [Test]
        public void WhenIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.When(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasCalled(m => m.When(Text, MultilineTextArg, table, null));
        }

        [Test]
        public void ThenIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.Then(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasCalled(m => m.Then(Text, MultilineTextArg, table, null));
        }

        [Test]
        public void ButIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.But(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasCalled(m => m.But(Text, MultilineTextArg, table, null));
        }

        [Test]
        public void AndIsDelegatedToObjectContainerCurrentTestRunner()
        {
            steps.And(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasCalled(m => m.And(Text, MultilineTextArg, table, null));
        }

        public class StepsTestableHelper : Steps
        {
            public StepsTestableHelper(ITestRunner testRunner) : base(testRunner)
            {
            }
        }
    }
}