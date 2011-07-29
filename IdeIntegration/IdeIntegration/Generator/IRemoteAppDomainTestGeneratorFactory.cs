using System;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public interface IRemoteAppDomainTestGeneratorFactory : ITestGeneratorFactory, IDisposable
    {
        bool IsRunning { get; }
        void Setup(string newGeneratorFolder);
        void EnsureInitialized();
        void Cleanup();
    }
}