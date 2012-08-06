using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoDi;
using TechTalk.SpecFlow.Generator.UnitTestProvider;

namespace TechTalk.SpecFlow.Generator
{
    partial class DefaultDependencyProvider
    {
        partial void RegisterUnitTestGeneratorProviders(ObjectContainer container)
        {
            container.RegisterTypeAs<NUnitTestGeneratorProvider, IUnitTestGeneratorProvider>("nunit");
            container.RegisterTypeAs<MbUnitTestGeneratorProvider, IUnitTestGeneratorProvider>("mbunit");
            container.RegisterTypeAs<MbUnit3TestGeneratorProvider, IUnitTestGeneratorProvider>("mbunit.3");
            container.RegisterTypeAs<XUnitTestGeneratorProvider, IUnitTestGeneratorProvider>("xunit");
            container.RegisterTypeAs<MsTestGeneratorProvider, IUnitTestGeneratorProvider>("mstest.2008");
            container.RegisterTypeAs<MsTest2010GeneratorProvider, IUnitTestGeneratorProvider>("mstest.2010");
            container.RegisterTypeAs<MsTest2010GeneratorProvider, IUnitTestGeneratorProvider>("mstest");

            container.RegisterTypeAs<MsTestSilverlightGeneratorProvider, IUnitTestGeneratorProvider>("mstest.silverlight");
            container.RegisterTypeAs<MsTestSilverlightGeneratorProvider, IUnitTestGeneratorProvider>("mstest.silverlight3");
            container.RegisterTypeAs<MsTestSilverlightGeneratorProvider, IUnitTestGeneratorProvider>("mstest.silverlight4");
            container.RegisterTypeAs<MsTestSilverlightGeneratorProvider, IUnitTestGeneratorProvider>("mstest.windowsphone7");
        }
    }
}
