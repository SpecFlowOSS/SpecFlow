using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ParserTests
{
    public static class TestFileHelper
    {
        public static IEnumerable<string> GetTestFiles()
        {
            string sampleFileFolder =
                Path.GetFullPath(
                    Path.Combine(
                        GetProjectLocation(),
                        "TestFiles"));

            foreach (var file in Directory.GetFiles(sampleFileFolder, "*.feature"))
            {
                yield return file;
            }
        }

        public static string GetAssemblyPath(Type type)
        {
            return new Uri(type.Assembly.CodeBase).AbsolutePath;
        }

        public static string GetProjectLocation()
        {
            string dllLocation = Path.GetDirectoryName(GetAssemblyPath(typeof(TestFileHelper)));
            return Path.Combine(dllLocation, @"..\..\..\ParserTests");
        }

        public static string ReadFile(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd();
            }
        }
    }
}