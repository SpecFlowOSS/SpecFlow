using System;
using System.Linq;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.CucumberMessages;
using TechTalk.SpecFlow.CucumberMessages.Sinks;
using TechTalk.SpecFlow.FileAccess;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class SinkProviderTests
    {
        [Fact(DisplayName = "When cucumber-messages are not configured, no sinks are returned")]
        public void GetMessageSinksFromConfiguration_Cucumber_Messages_Are_Not_Configured__No_Sink()
        {
            var specFlowConfiguration = ConfigurationLoader.GetDefault();

            var sinkProvider = new SinkProvider(specFlowConfiguration, new Mock<IBinaryFileAccessor>().Object, new Mock<IProtobufFileNameResolver>().Object);

            sinkProvider.GetMessageSinksFromConfiguration().Should().BeEmpty();
        }

        [Fact(DisplayName = "When cucumber-messages are disabled, no sinks are returned")]
        public void GetMessageSinksFromConfiguration_Cucumber_Messages_Are_Disabled__No_Sink()
        {
            var specFlowConfiguration = ConfigurationLoader.GetDefault();
            specFlowConfiguration.CucumberMessagesConfiguration.Enabled = false;

            var sinkProvider = new SinkProvider(specFlowConfiguration, new Mock<IBinaryFileAccessor>().Object, new Mock<IProtobufFileNameResolver>().Object);

            sinkProvider.GetMessageSinksFromConfiguration().Should().BeEmpty();
        }


        [Fact(DisplayName = "When cucumber-messages are enabled, but nothing else configured, the default sink is returned")]
        public void GetMessageSinksFromConfiguration_Cucumber_Messages_Are_Enabled__Default_Sink()
        {
            var specFlowConfiguration = ConfigurationLoader.GetDefault();
            specFlowConfiguration.CucumberMessagesConfiguration.Enabled = true;

            var sinkProvider = new SinkProvider(specFlowConfiguration, new Mock<IBinaryFileAccessor>().Object, new Mock<IProtobufFileNameResolver>().Object);

            sinkProvider.GetMessageSinksFromConfiguration().Should().HaveCount(1);
            var cucumberMessageSink = sinkProvider.GetMessageSinksFromConfiguration().First();
            cucumberMessageSink.Should().BeOfType<ProtobufFileSink>();
        }

        [Fact(DisplayName = "When cucumber-messages sinks are configured, these are used")]
        public void GetMessageSinksFromConfiguration_CustomSinks_are_configured__These_are_returned()
        {
            var specFlowConfiguration = ConfigurationLoader.GetDefault();
            specFlowConfiguration.CucumberMessagesConfiguration.Enabled = true;

            specFlowConfiguration.CucumberMessagesConfiguration.Sinks.Add(new CucumberMessagesSink("file", "path_to_something"));
            specFlowConfiguration.CucumberMessagesConfiguration.Sinks.Add(new CucumberMessagesSink("file", "path_to_something_else"));

            var sinkProvider = new SinkProvider(specFlowConfiguration, new Mock<IBinaryFileAccessor>().Object, new Mock<IProtobufFileNameResolver>().Object);

            var messageSinksFromConfiguration = sinkProvider.GetMessageSinksFromConfiguration();
            messageSinksFromConfiguration.Should().HaveCount(2);
            messageSinksFromConfiguration.Should().AllBeOfType<ProtobufFileSink>();
        }

        [Fact(DisplayName = "When unknown cucumber-messages sinks are configured, there is an error")]
        public void GetMessageSinksFromConfiguration_Unknown_sinks_are_configured__Error_is_thrown()
        {
            var specFlowConfiguration = ConfigurationLoader.GetDefault();
            specFlowConfiguration.CucumberMessagesConfiguration.Enabled = true;

            specFlowConfiguration.CucumberMessagesConfiguration.Sinks.Add(new CucumberMessagesSink("unknown_type", "path_to_something"));
            
            var sinkProvider = new SinkProvider(specFlowConfiguration, new Mock<IBinaryFileAccessor>().Object, new Mock<IProtobufFileNameResolver>().Object);

            Action action = () => sinkProvider.GetMessageSinksFromConfiguration();
            action.Should().Throw<NotImplementedException>().WithMessage("The sink type unknown_type");

        }
    }
}