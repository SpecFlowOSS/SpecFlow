using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.Specs.Support
{
    public class TestFileManager
    {
        public string GetTestFileContent(string testfileName)
        {
            var projectTemplateStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TechTalk.SpecFlow.Specs.TestFiles." + testfileName);
            Debug.Assert(projectTemplateStream != null);
            string fileContent = new StreamReader(projectTemplateStream).ReadToEnd();
            return fileContent;
        }

        public IEnumerable<string> GetTestFeatureFiles()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string prefixToRemove = "TechTalk.SpecFlow.Specs.TestFiles.";
            return assembly.GetManifestResourceNames()
                .Where(rn => rn.EndsWith(".feature") && rn.StartsWith(prefixToRemove))
                .Select(rn => rn.Substring(prefixToRemove.Length));
        }
    }
}
