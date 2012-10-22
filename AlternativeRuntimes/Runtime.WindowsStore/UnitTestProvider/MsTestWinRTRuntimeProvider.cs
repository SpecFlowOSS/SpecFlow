// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsTestWinRTRuntimeProvider.cs" company="blinkBox Entertainment Ltd.">
//   Copyright (c) blinkBox Entertainment Ltd. All rights reserved.
// </copyright>
// <summary>
//   The ms test win rt runtime provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TechTalk.SpecFlow.UnitTestProvider
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using TechTalk.SpecFlow;

    #endregion

    /// <summary>
    /// The ms test win rt runtime provider.
    /// </summary>
    internal class MsTestWinRTRuntimeProvider : IUnitTestRuntimeProvider
    {
        #region Constants

        /// <summary>
        /// The asser t_ type.
        /// </summary>
        private const string ASSERT_TYPE = "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert";

        /// <summary>
        /// The mstes t_ assembly.
        /// </summary>
        private const string MSTEST_ASSEMBLY = "Microsoft.VisualStudio.TestPlatform.UnitTestFramework, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        #endregion

        #region Fields

        /// <summary>
        /// The assert inconclusive.
        /// </summary>
        private Action<string, object[]> assertInconclusive;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the assembly name.
        /// </summary>
        public virtual string AssemblyName
        {
            get
            {
                return MSTEST_ASSEMBLY;
            }
        }

        /// <summary>
        /// Gets a value indicating whether delayed fixture tear down.
        /// </summary>
        public bool DelayedFixtureTearDown
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Methods and Operators


        /// <summary>
        /// The test ignore.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void TestIgnore(string message)
        {
            this.TestInconclusive(message); // there is no dynamic "Ignore" in mstest
        }

        /// <summary>
        /// The test inconclusive.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void TestInconclusive(string message)
        {
            if (assertInconclusive == null)
            {
                assertInconclusive = UnitTestRuntimeProviderHelper.GetAssertMethodWithFormattedMessage(AssemblyName, ASSERT_TYPE, "Inconclusive");
            }

            assertInconclusive("{0}", new object[] { message });
        }

        /// <summary>
        /// The test pending.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void TestPending(string message)
        {
            this.TestInconclusive(message);
        }

        #endregion
    }
}