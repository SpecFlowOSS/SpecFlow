﻿using System.IO;
using SpecFlow.Internal.Json;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace SpecFlow.ExternalData.SpecFlowPlugin
{
    public interface ITestDataProvider
    {
        dynamic TestData { get; }
    }

    public class TestDataProvider : ITestDataProvider
    {
        public TestDataProvider(ProjectSettings projectSettings)
        {
            var testDataContent = File.ReadAllText(Path.Combine(projectSettings.ProjectFolder, "testdata.json"));
            TestData = testDataContent.FromJson<dynamic>();
        }

        public dynamic TestData { get; }
    }
}
