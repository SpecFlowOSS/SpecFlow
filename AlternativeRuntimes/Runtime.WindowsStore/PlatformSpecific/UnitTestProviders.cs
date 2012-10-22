﻿
using BoDi;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Infrastructure
{
    partial class DefaultDependencyProvider
    {
        partial void RegisterUnitTestProviders(ObjectContainer container)
        {
            container.RegisterTypeAs<MsTestWinRTRuntimeProvider, IUnitTestRuntimeProvider>("mstest.winRT");
        }
    }
}
