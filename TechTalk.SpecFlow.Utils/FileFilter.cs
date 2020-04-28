using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TechTalk.SpecFlow.Utils
{
    public static class FileFilter
    {
        public static IReadOnlyCollection<string> GetValidFiles(IReadOnlyCollection<string> filePaths)
        {
            var valids = filePaths.Where(ValidFile).ToList();
            return valids;
        }

        private static bool ValidFile(string filePath)
        {
            try
            {
                return ValidateFileName(filePath) && ValidateFilePath(filePath);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool ValidateFileName(string filePath)
        {
            var invalidFileNameChars = Path.GetInvalidFileNameChars();
            var fileName = Path.GetFileName(filePath);

            return !string.IsNullOrEmpty(fileName) &&
                   !fileName.Any(s => invalidFileNameChars.Contains(s));
        }

        private static bool ValidateFilePath(string filePath)
        {
            string pathWithoutSeparator = filePath
                                          .Replace(Path.DirectorySeparatorChar.ToString(), string.Empty)
                                          .Replace(Path.AltDirectorySeparatorChar.ToString(), string.Empty)
                                          .Replace(Path.VolumeSeparatorChar.ToString(), string.Empty);
            var invalidPathChars = Path.GetInvalidPathChars();

            return !string.IsNullOrEmpty(pathWithoutSeparator) &&
                   !pathWithoutSeparator.Any(invalidPathChars.Contains);
        }

    }
}
