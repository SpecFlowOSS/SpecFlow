using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Utils
{
    public static class FileSystemHelper
    {
        public static void CopyFileToFolder(string filePath, string folderName)
        {
            File.Copy(filePath, Path.Combine(folderName, Path.GetFileName(filePath)));
        }

        public static string GetRelativePath(string path, string basePath)
        {
            path = Path.GetFullPath(path);
            basePath = Path.GetFullPath(basePath);
            if (String.Equals(path, basePath, StringComparison.OrdinalIgnoreCase))
                return "."; // the "this folder"

            if (path.StartsWith(basePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                return path.Substring(basePath.Length + 1);

            //handle different drives
            string pathRoot = Path.GetPathRoot(path);
            if (!String.Equals(pathRoot, Path.GetPathRoot(basePath), StringComparison.OrdinalIgnoreCase))
                return path;

            //handle ".." pathes
            string[] pathParts = path.Substring(pathRoot.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string[] basePathParts = basePath.Substring(pathRoot.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            int commonFolderCount = 0;
            while (commonFolderCount < pathParts.Length && commonFolderCount < basePathParts.Length &&
                   String.Equals(pathParts[commonFolderCount], basePathParts[commonFolderCount], StringComparison.OrdinalIgnoreCase))
                commonFolderCount++;

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < basePathParts.Length - commonFolderCount; i++)
            {
                result.Append("..");
                result.Append(Path.DirectorySeparatorChar);
            }

            if (pathParts.Length - commonFolderCount == 0)
                return result.ToString().TrimEnd(Path.DirectorySeparatorChar);

            result.Append(String.Join(Path.DirectorySeparatorChar.ToString(), pathParts, commonFolderCount, pathParts.Length - commonFolderCount));
            return result.ToString();
        }

        // This method accepts two strings the represent two files to 
        // compare. A return value of true indicates that the contents of the files
        // are the same. A return value of any other value indicates that the 
        // files are not the same.
        public static bool FileCompare(string filePath1, string filePath2)
        {
            int file1byte;
            int file2byte;

            // Determine if the same file was referenced two times.
            if (String.Equals(filePath1, filePath2, StringComparison.CurrentCultureIgnoreCase))
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            using (FileStream fs1 = new FileStream(filePath1, FileMode.Open, FileAccess.Read))
            using (FileStream fs2 = new FileStream(filePath2, FileMode.Open, FileAccess.Read))
            {
                // Check the file sizes. If they are not the same, the files 
                // are not the same.
                if (fs1.Length != fs2.Length)
                {
                    // Return false to indicate files are different
                    return false;
                }

                // Read and compare a byte from each file until either a
                // non-matching set of bytes is found or until the end of
                // file1 is reached.
                do
                {
                    // Read one byte from each file.
                    file1byte = fs1.ReadByte();
                    file2byte = fs2.ReadByte();
                } while ((file1byte == file2byte) && (file1byte != -1));
            }

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }
    }
}
