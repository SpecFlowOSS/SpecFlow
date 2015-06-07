using System;
using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class ServiceTests
    {
        [Test]
        public void Should_load_the_value_comparers_by_default()
        {
            var service = new Service();

            var results = service.ValueComparers;
            Assert.AreEqual(7, results.Count());

            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(DateTimeValueComparer)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(BoolValueComparer)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(GuidValueComparer)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(DecimalValueComparer)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(DoubleValueComparer)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(FloatValueComparer)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(DefaultValueComparer)).Count());
        }
    }
}