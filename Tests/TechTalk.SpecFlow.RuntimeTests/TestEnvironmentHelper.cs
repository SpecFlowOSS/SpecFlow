using System;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class TestEnvironmentHelper
    {
        public static bool IsBeingRunByNCrunch()
        {
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentVariableNames.NCrunch));
        }
    }
}