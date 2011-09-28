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
            container.RegisterTypeAs<MsTestSilverlightRuntimeProvider, IUnitTestRuntimeProvider>("mstest.silverlight");
            container.RegisterTypeAs<MsTestSilverlightRuntimeProvider, IUnitTestRuntimeProvider>("mstest.silverlight3");
            container.RegisterTypeAs<MsTestSilverlightRuntimeProvider, IUnitTestRuntimeProvider>("mstest.silverlight4");
        }
    }
}
