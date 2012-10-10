using System;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public interface IGeneratorServices : IDisposable
    {
        void InvalidateSettings();
        ITestGenerator CreateTestGenerator();
        ITestGenerator CreateTestGeneratorOfIDE();
        Version GetGeneratorVersion();
    }
}