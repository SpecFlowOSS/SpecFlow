using System;
using System.IO;

namespace TechTalk.SpecFlow.Analytics.UserId
{
    public class FileUserIdStore : IUserUniqueIdStore
    {
        private static readonly string appDataFolder = Environment.GetFolderPath(
                                                                    Environment.SpecialFolder.ApplicationData);
        public static readonly string UserIdFilePath = Path.Combine(appDataFolder, "SpecFlow", "userid");

        private readonly Lazy<string> _lazyUniqueUserId;
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;

        public FileUserIdStore(IFileService fileService, IDirectoryService directoryService)
        {
            _fileService = fileService;
            _directoryService = directoryService;
            _lazyUniqueUserId = new Lazy<string>(FetchAndPersistUserId);
        }

        public string GetUserId()
        {
            return _lazyUniqueUserId.Value;
        }

        private string FetchAndPersistUserId()
        {
            if (_fileService.Exists(UserIdFilePath))
            {
                var userIdStringFromFile = _fileService.ReadAllText(UserIdFilePath);
                if (IsValidGuid(userIdStringFromFile))
                {
                    return userIdStringFromFile;
                }
            }

            return GenerateAndPersistUserId();
        }

        private void PersistUserId(string userId)
        {
            var directoryName = Path.GetDirectoryName(UserIdFilePath);
            if (!_directoryService.Exists(directoryName))
            {
                _directoryService.CreateDirectory(directoryName);
            }

            _fileService.WriteAllText(UserIdFilePath, userId);
        }

        private bool IsValidGuid(string guid)
        {
            return Guid.TryParse(guid, out var parsedGuid);
        }

        private string GenerateAndPersistUserId()
        {
            var newUserId = Guid.NewGuid().ToString();

            PersistUserId(newUserId);

            return newUserId;
        }
    }
}
