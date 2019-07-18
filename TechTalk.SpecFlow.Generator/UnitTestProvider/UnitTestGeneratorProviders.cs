﻿using BoDi;
using TechTalk.SpecFlow.Generator.UnitTestProvider;

namespace TechTalk.SpecFlow.Generator
{
    partial class DefaultDependencyProvider
    {
        partial void RegisterUnitTestGeneratorProviders(ObjectContainer container)
        {
            container.RegisterTypeAs<NUnitTestGeneratorProvider, IUnitTestGeneratorProvider>("nunit.2");
            container.RegisterTypeAs<NUnit3TestGeneratorProvider, IUnitTestGeneratorProvider>("nunit");
            container.RegisterTypeAs<XUnitTestGeneratorProvider, IUnitTestGeneratorProvider>("xunit.1");
            container.RegisterTypeAs<XUnit2TestGeneratorProvider, IUnitTestGeneratorProvider>("xunit");
            container.RegisterTypeAs<MsTestV2GeneratorProvider, IUnitTestGeneratorProvider>("mstest");
            container.RegisterTypeAs<MsTest2010GeneratorProvider, IUnitTestGeneratorProvider>("mstest.v1");
        }
    }
}
