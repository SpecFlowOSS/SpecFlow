using Moq;
using TechTalk.SpecFlow.CucumberMessages;
using TechTalk.SpecFlow.ErrorHandling;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultFactoryTestsBase
    {
        public TestResultFactory GetTestResultFactory(Mock<IErrorProvider> errorProviderMock = null, Mock<ITestUndefinedMessageFactory> testUndefinedMessageFactoryMock = null)
        {
            var testResultFactory = new TestResultFactory(
                new TestErrorMessageFactory(),
                new TestPendingMessageFactory(errorProviderMock?.Object ?? GetErrorProviderMock().Object),
                new TestAmbiguousMessageFactory(),
                testUndefinedMessageFactoryMock?.Object ?? GetTestUndefinedMessageFactoryMock().Object);
            return testResultFactory;
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