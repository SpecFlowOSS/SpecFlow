using System;
using System.Linq;

namespace TechTalk.SpecFlow.UnitTestProvider
{
	public class XUnitRuntimeProvider : IUnitTestRuntimeProvider
	{
		private const string XUNIT_ASSEMBLY = "xunit";
		private const string ASSERT_TYPE = "Xunit.Assert";

		Action<string> assertInconclusive = null;
		Action<string> assertIgnore = null;

		public void TestInconclusive(string message)
		{
			throw new NotSupportedException("XUnit does not support Assert.Inconclusive");
		}

		public void TestIgnore(string message)
		{
			throw new NotSupportedException("XUnit does not support Assert.Ignore");
		}

		public bool DelayedFixtureTearDown
		{
			get { return false; }
		}
	}
}