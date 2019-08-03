using Moq;
using TechTalk.SpecFlow.CucumberMessages;
using TechTalk.SpecFlow.ErrorHandling;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultPartFactoryTestsBase
    {
        public TestResultPartsFactory GetTestResultPartFactory(string expectedMessage = null)
        {
            var testResultFactory = new TestResultPartsFactory(
                CreateTestErrorMessageFactoryMock(expectedMessage),
                CreateTestPendingMessageFactoryMock(expectedMessage),
                CreateTestAmbiguousMessageFactoryMock(expectedMessage),
                CreateTestUndefiendMessageFactoryMock(expectedMessage));
            return testResultFactory;
        }

        private ITestUndefinedMessageFactory CreateTestUndefiendMessageFactoryMock(string expectedMessage)
        {
            var mock = new Mock<ITestUndefinedMessageFactory>();
            mock.Setup(m => m.BuildFromContext(It.IsAny<ScenarioContext>(), It.IsAny<FeatureContext>())).Returns(() => expectedMessage);
            return mock.Object;
        }

        private ITestPendingMessageFactory CreateTestPendingMessageFactoryMock(string expectedMessage)
        {
            var mock = new Mock<ITestPendingMessageFactory>();
            mock.Setup(m => m.BuildFromScenarioContext(It.IsAny<ScenarioContext>())).Returns(() => expectedMessage);
            return mock.Object;
        }

        private ITestErrorMessageFactory CreateTestErrorMessageFactoryMock(string expectedMessage)
        {
            var mock = new Mock<ITestErrorMessageFactory>();
            mock.Setup(m => m.BuildFromScenarioContext(It.IsAny<ScenarioContext>())).Returns(() => expectedMessage);

            return mock.Object;
        }

        private ITestAmbiguousMessageFactory CreateTestAmbiguousMessageFactoryMock(string expectedMessage)
        {
            var mock = new Mock<ITestAmbiguousMessageFactory>();
            mock.Setup(m => m.BuildFromScenarioContext(It.IsAny<ScenarioContext>())).Returns(() => expectedMessage);
            return mock.Object;
        }

        public Mock<IErrorProvider> GetErrorProviderMock()
        {
            var errorProviderMock = new Mock<IErrorProvider>();
            errorProviderMock.Setup(m => m.GetPendingStepDefinitionError())
                .Returns(new PendingStepException());
            return errorProviderMock;
        }

        public Mock<ITestUndefinedMessageFactory> GetTestUndefinedMessageFactoryMock()
        {
            var testUndefinedMessageFactoryMock = new Mock<ITestUndefinedMessageFactory>();
            testUndefinedMessageFactoryMock.Setup(m => m.BuildFromContext(It.IsAny<ScenarioContext>(), It.IsAny<FeatureContext>()))
                .Returns("Step definition not defined");
            return testUndefinedMessageFactoryMock;
        }
    }
}