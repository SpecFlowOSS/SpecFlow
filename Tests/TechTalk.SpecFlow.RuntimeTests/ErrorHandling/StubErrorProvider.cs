using System;
using Moq;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.RuntimeTests.ErrorHandling
{
    internal class StubErrorProvider : ErrorProvider
    {
        public StubErrorProvider() : 
            base(new StepFormatter(new ColorOutputHelper(ConfigurationLoader.GetDefault()), new ColorOutputTheme()), ConfigurationLoader.GetDefault(), GetStubUnitTestProvider())
        {
        }

        private static IUnitTestRuntimeProvider GetStubUnitTestProvider()
        {
            var mock = new Mock<IUnitTestRuntimeProvider>();
            mock.Setup(utp => utp.TestIgnore(It.IsAny<string>())).Throws<InvalidOperationException>();
            mock.Setup(utp => utp.TestInconclusive(It.IsAny<string>())).Throws<InvalidOperationException>();
            mock.Setup(utp => utp.TestPending(It.IsAny<string>())).Throws<InvalidOperationException>();
            return mock.Object;
        }
    }
}
