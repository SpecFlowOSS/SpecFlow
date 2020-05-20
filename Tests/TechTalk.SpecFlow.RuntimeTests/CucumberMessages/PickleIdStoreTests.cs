using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.CucumberMessages;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class PickleIdStoreTests
    {
        [Fact(DisplayName = @"GetPickleIdForScenarioInfo should add a Pickle ID to the dictionary if it does not already exist")]
        public void GetPickleIdForScenarioInfo_ScenarioInfo_ShouldAddPickleIdToDictionaryIfNotExistent()
        {
            // ARRANGE
            var dictionary = new Dictionary<ScenarioInfo, Guid>();
            var pickleIdStoreDictionaryFactoryMock = GetPickleIdStoreDictionaryFactoryMock(dictionary);
            var guidToCreate = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            var scenarioInfo = new ScenarioInfo("Title", "Description", null, null);
            var mock = GetPickleIdGeneratorMock(guidToCreate);

            var pickleIdStore = new PickleIdStore(mock.Object, pickleIdStoreDictionaryFactoryMock.Object);

            // ACT
            pickleIdStore.GetPickleIdForScenario(scenarioInfo);

            // ASSERT
            dictionary.Should().Contain(scenarioInfo, guidToCreate);
        }

        [Fact(DisplayName = @"GetPickleIdForScenarioInfo should return the Pickle ID if it already exists")]
        public void GetPickleIdForScenarioInfo_ScenarioInfo_ShouldReturnPickleIdIfAlreadyExists()
        {
            // ARRANGE
            var existingGuid = new Guid(11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1);
            var guidToCreate = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            var scenarioInfo = new ScenarioInfo("Title", "Description", null, null);
            var dictionary = new Dictionary<ScenarioInfo, Guid> { [scenarioInfo] = existingGuid };
            var pickleIdStoreDictionaryFactoryMock = GetPickleIdStoreDictionaryFactoryMock(dictionary);
            var mock = GetPickleIdGeneratorMock(guidToCreate);

            var pickleIdStore = new PickleIdStore(mock.Object, pickleIdStoreDictionaryFactoryMock.Object);

            // ACT
            var actualReturnedGuid = pickleIdStore.GetPickleIdForScenario(scenarioInfo);

            // ASSERT
            actualReturnedGuid.Should().Be(existingGuid);
        }

        [Fact(DisplayName = @"GetPickleIdForScenarioInfo should not overwrite the Pickle ID for already existing entries")]
        public void GetPickleIdForScenarioInfo_ScenarioInfo_ShouldNotOverwritePickleIdIfAlreadyExists()
        {
            // ARRANGE
            var existingGuid = new Guid(11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1);
            var guidToCreate = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            var scenarioInfo = new ScenarioInfo("Title", "Description", null, null);
            var dictionary = new Dictionary<ScenarioInfo, Guid> { [scenarioInfo] = existingGuid };
            var pickleIdStoreDictionaryFactoryMock = GetPickleIdStoreDictionaryFactoryMock(dictionary);
            var mock = GetPickleIdGeneratorMock(guidToCreate);

            var pickleIdStore = new PickleIdStore(mock.Object, pickleIdStoreDictionaryFactoryMock.Object);

            // ACT
            pickleIdStore.GetPickleIdForScenario(scenarioInfo);

            // ASSERT
            dictionary.Should().Contain(scenarioInfo, existingGuid)
                      .And.NotContain(scenarioInfo, guidToCreate);
        }

        public Mock<IPickleIdStoreDictionaryFactory> GetPickleIdStoreDictionaryFactoryMock(Dictionary<ScenarioInfo, Guid> dictionary)
        {
            var pickleIdStoreDictionaryFactoryMock = new Mock<IPickleIdStoreDictionaryFactory>();
            pickleIdStoreDictionaryFactoryMock.Setup(m => m.BuildDictionary())
                                              .Returns(dictionary);
            return pickleIdStoreDictionaryFactoryMock;
        }

        public Mock<IPickleIdGenerator> GetPickleIdGeneratorMock(Guid guidToCreate)
        {
            var mock = new Mock<IPickleIdGenerator>();
            mock.Setup(m => m.GeneratePickleId())
                .Returns(guidToCreate);
            return mock;
        }
    }
}
