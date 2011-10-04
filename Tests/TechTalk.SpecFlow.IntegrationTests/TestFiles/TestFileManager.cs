using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TechTalk.SpecFlow.IntegrationTests.TestFiles
{
    public class TestFileManager
    {
        public string GetTestFileContent(string testfileName)
        {
            var projectTemplateStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetType(), testfileName);
            Debug.Assert(projectTemplateStream != null);
            string fileContent = new StreamReader(projectTemplateStream).ReadToEnd();
            return fileContent;
        }

        public IEnumerable<string> GetTestFeatureFiles()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string prefixToRemove = GetType().Namespace + ".";
            return assembly.GetManifestResourceNames()
                .Where(rn => rn.EndsWith(".feature") && rn.StartsWith(prefixToRemove))
                .Select(rn => rn.Substring(prefixToRemove.Length));
        }
    }
}
