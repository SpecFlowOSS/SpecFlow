﻿using System;
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
            container.RegisterTypeAs<NUnitRuntimeProvider, IUnitTestRuntimeProvider>("nunit");
            container.RegisterTypeAs<MbUnitRuntimeProvider, IUnitTestRuntimeProvider>("mbunit");
            container.RegisterTypeAs<MbUnit3RuntimeProvider, IUnitTestRuntimeProvider>("mbunit.3");
            container.RegisterTypeAs<XUnitRuntimeProvider, IUnitTestRuntimeProvider>("xunit");
            container.RegisterTypeAs<MsTestRuntimeProvider, IUnitTestRuntimeProvider>("mstest");
            container.RegisterTypeAs<MsTestWithMolesRuntimeProvider, IUnitTestRuntimeProvider>("mstest.moles");
            container.RegisterTypeAs<MsTest2010RuntimeProvider, IUnitTestRuntimeProvider>("mstest.2010");
        }
    }
}
