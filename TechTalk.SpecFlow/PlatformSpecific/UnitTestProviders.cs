using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoDi;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Infrastructure
{
    partial class DefaultDependencyProvider
    {
        partial void RegisterUnitTestProviders(ObjectContainer container)
        {
            container.RegisterTypeAs<NUnitRuntimeProvider, IUnitTestRuntimeProvider>("nunit.2");
            container.RegisterTypeAs<NUnit3RuntimeProvider, IUnitTestRuntimeProvider>("nunit");
            container.RegisterTypeAs<MbUnitRuntimeProvider, IUnitTestRuntimeProvider>("mbunit");
            container.RegisterTypeAs<MbUnit3RuntimeProvider, IUnitTestRuntimeProvider>("mbunit.3");
            container.RegisterTypeAs<XUnitRuntimeProvider, IUnitTestRuntimeProvider>("xunit.1");
            container.RegisterTypeAs<XUnit2RuntimeProvider, IUnitTestRuntimeProvider>("xunit");
            container.RegisterTypeAs<MsTestRuntimeProvider, IUnitTestRuntimeProvider>("mstest.2008");
            container.RegisterTypeAs<MsTest2010RuntimeProvider, IUnitTestRuntimeProvider>("mstest.2010");
            container.RegisterTypeAs<MsTest2010RuntimeProvider, IUnitTestRuntimeProvider>("mstest");
            container.RegisterTypeAs<MsTestV2RuntimeProvider, IUnitTestRuntimeProvider>("mstest.v2");
        }
    }
}
