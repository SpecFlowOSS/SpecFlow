using System;
using System.Linq;

namespace TechTalk.SpecFlow.Generator
{
    public interface ITestHeaderWriter
    {
        Version DetectGeneratedTestVersion(string generatedTestContent);
    }
}
