using System.Globalization;
using System.Threading;
using TechTalk.SpecFlow.Configuration;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings
{
    public class CultureInfoScopeTests
    {
        [Fact]
        public void DoesNothing_WhenContextIsNull()
        {
            FeatureContext context = null;

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            using (new CultureInfoScope(context))
            {
                Assert.Equal(currentCulture, Thread.CurrentThread.CurrentCulture);
            }
            Assert.Equal(currentCulture, Thread.CurrentThread.CurrentCulture);
        }

        [Fact]
        public void DoesNothing_WhenCultureIsCurrent()
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            FeatureContext context = GetFeatureContext(currentCulture);

            using (new CultureInfoScope(context))
            {
                Assert.Equal(currentCulture, Thread.CurrentThread.CurrentCulture);
            }
            Assert.Equal(currentCulture, Thread.CurrentThread.CurrentCulture);
        }

        [Fact]
        public void SwapsCultureInfo_WhenCultureDifferent()
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var replaceCulture = new CultureInfo("IT-it");
            FeatureContext context = GetFeatureContext(replaceCulture);

            using (new CultureInfoScope(context))
            {
                Assert.Equal(replaceCulture, Thread.CurrentThread.CurrentCulture);
            }
            Assert.Equal(currentCulture, Thread.CurrentThread.CurrentCulture);
        }

        private static FeatureContext GetFeatureContext(CultureInfo cultureInfo)
        {
            return new FeatureContext(default,
                                      new FeatureInfo(cultureInfo, default, default, default),
                                      new SpecFlowConfiguration(default, default, default, default, cultureInfo, default, default, default, default, default, default, default, default, default, default, default, default));
        }
    }
}
