using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Google.Protobuf;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.TestProjectGenerator;

namespace TechTalk.SpecFlow.Specs.Drivers.CucumberMessages
{
    public class CucumberMessagesDriver
    {
        private readonly TestProjectFolders _testProjectFolders;

        public CucumberMessagesDriver(TestProjectFolders testProjectFolders)
        {
            _testProjectFolders = testProjectFolders;
        }

        public IMessage UnpackWrapper(Wrapper wrapper)
        {
            switch (wrapper.MessageCase)
            {
                case Wrapper.MessageOneofCase.TestRunStarted: return wrapper.TestRunStarted;
                case Wrapper.MessageOneofCase.TestCaseStarted: return wrapper.TestCaseStarted;
                default: throw new InvalidOperationException($"(Currently) unsupported message type: {wrapper.MessageCase}");
            }
        }

        public IEnumerable<IMessage> LoadMessageQueue()
        {
            string pathToCucumberMessagesFile = Path.Combine(_testProjectFolders.ProjectBinOutputPath, "CucumberMessageQueue", "messages");
            using (var fileStream = File.Open(pathToCucumberMessagesFile, FileMode.Open, System.IO.FileAccess.Read))
            {
                while (fileStream.CanSeek && fileStream.Position < fileStream.Length)
                {
                    var messageParser = Wrapper.Parser;
                    var message = messageParser.ParseDelimitedFrom(fileStream);
                    yield return UnpackWrapper(message);
                }
            }
        }

        public void TestRunStartedMessageShouldHaveBeenSent()
        {
            var messageQueue = LoadMessageQueue();
            messageQueue.ToArray().Should().Contain(m => m is TestRunStarted);
        }

        public void TestRunStartedMessageShouldHaveBeenSent(Table values)
        {
            var messageQueue = LoadMessageQueue();
            var testRunStarted = messageQueue.ToArray().Should().Contain(m => m is TestRunStarted)
                                             .Which.Should().BeOfType<TestRunStarted>()
                                             .Which;

            values.CompareToInstance(testRunStarted);
        }
    }
}
