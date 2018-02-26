using System.IO;

namespace TechTalk.SpecFlow.Generator.Helpers
{
    public static class PathHelper
    {
        public static string SanitizeDirectorySeparatorChar(string path)
        {
            return path.Replace(@"\", $"{Path.DirectorySeparatorChar}").Replace(@"/", $"{Path.DirectorySeparatorChar}");
        }
    }
}
