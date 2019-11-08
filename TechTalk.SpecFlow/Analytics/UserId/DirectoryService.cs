using System.IO;

namespace TechTalk.SpecFlow.Analytics.UserId
{
    public class DirectoryService : IDirectoryService
    {
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
