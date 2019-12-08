using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.Analytics.UserId;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests.MSBuildTask
{
    public class FileUserIdStoreTests
    {
        private const string UserId = "491ed5c0-9f25-4c27-941a-19b17cc81c87";
        
        Mock<IFileService> fileServiceStub;
        Mock<IDirectoryService> directoryServiceStub;
        FileUserIdStore sut;

        public FileUserIdStoreTests()
        {
            fileServiceStub = new Mock<IFileService>();
            directoryServiceStub = new Mock<IDirectoryService>();
            sut = new FileUserIdStore(fileServiceStub.Object, directoryServiceStub.Object);
        }

        private void GivenUserIdStringInFile(string userIdString)
        {
            fileServiceStub.Setup(fileService => fileService.ReadAllText(It.IsAny<string>())).Returns(userIdString);
        }

        private void GivenFileExists()
        {
            fileServiceStub.Setup(fileService => fileService.Exists(It.IsAny<string>())).Returns(true);
        }

        private void GivenFileDoesNotExists()
        {
            fileServiceStub.Setup(fileService => fileService.Exists(It.IsAny<string>())).Returns(false);
        }

        [Fact]
        public void Should_GetUserIdFromFile_WhenFileExists()
        {
            GivenFileExists();
            GivenUserIdStringInFile(UserId);

            string userId = sut.GetUserId();

            userId.Should().Be(UserId);
        }

        [Fact]
        public void Should_GenerateNewUserId_WhenFileDoesNotExists()
        {
            GivenFileDoesNotExists();

            string userId = sut.GetUserId();

            userId.Should().NotBeEmpty();
        }

        [Fact]
        public void Should_PersistNewlyGeneratedUserId_WhenNoUserIdExists()
        {
            GivenFileDoesNotExists();

            string userId = sut.GetUserId();

            userId.Should().NotBeEmpty();
            fileServiceStub.Verify(fileService => fileService.WriteAllText(FileUserIdStore.UserIdFilePath, userId), Times.Once());
        }
    }
}
