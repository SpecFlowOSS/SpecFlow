
using System;
using System.IO;
using System.Text;

namespace TechTalk.SpecFlow.SharpDevelop4Integration
{
	//TODO: This should probably be in a commono project since it's could be used by multiple 
	// projects (i.e. SpecFlowSingleFileGeneratorBase) as well.
	public static class FileUtilities
	{
		public static string GetRelativePath(string path, string basePath)
        {
            path = Path.GetFullPath(path);
            basePath = Path.GetFullPath(basePath);
            if (string.Equals(path, basePath, StringComparison.OrdinalIgnoreCase))
                return "."; // the "this folder"

            if (path.StartsWith(basePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                return path.Substring(basePath.Length + 1);

            //handle different drives
            string pathRoot = Path.GetPathRoot(path);
            if (!string.Equals(pathRoot, Path.GetPathRoot(basePath), StringComparison.OrdinalIgnoreCase))
                return path;

            //handle ".." pathes
            string[] pathParts = path.Substring(pathRoot.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string[] basePathParts = basePath.Substring(pathRoot.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            int commonFolderCount = 0;
            while (commonFolderCount < pathParts.Length && commonFolderCount < basePathParts.Length &&
                string.Equals(pathParts[commonFolderCount], basePathParts[commonFolderCount], StringComparison.OrdinalIgnoreCase))
                commonFolderCount++;

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < basePathParts.Length - commonFolderCount; i++)
            {
                result.Append("..");
                result.Append(Path.DirectorySeparatorChar);
            }

            if (pathParts.Length - commonFolderCount == 0)
                return result.ToString().TrimEnd(Path.DirectorySeparatorChar);

            result.Append(string.Join(Path.DirectorySeparatorChar.ToString(), pathParts, commonFolderCount, pathParts.Length - commonFolderCount));
            return result.ToString();
        }
	}
}
