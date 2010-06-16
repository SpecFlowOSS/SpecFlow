using System;
using System.Diagnostics;
using System.Linq;

#if SILVERLIGHT
using TechTalk.SpecFlow.Compatibility;
#endif

namespace TechTalk.SpecFlow
{
    public class FeatureContext : SpecFlowContext
    {
        static public FeatureContext Current
        {
            get { return ObjectContainer.FeatureContext; }
        }

        public FeatureInfo FeatureInfo { get; private set; }
        internal Stopwatch Stopwatch { get; private set; }

        public FeatureContext(FeatureInfo featureInfo)
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();

            FeatureInfo = featureInfo;
        }
    }
}