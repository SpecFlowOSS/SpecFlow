using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using TechTalk.SpecFlow.Infrastructure;

#if SILVERLIGHT
using TechTalk.SpecFlow.Compatibility;
#endif

namespace TechTalk.SpecFlow
{
    public class ScenarioContext : SpecFlowContext
    {
        private static ScenarioContext current;
        public static ScenarioContext Current
        {
            get
            {
                if (current == null)
                {
                    Debug.WriteLine("Accessing NULL ScenarioContext");
                }
                return current;
            }
            internal set { current = value; }
        }

        public ScenarioInfo ScenarioInfo { get; private set; }
        public ScenarioBlock CurrentScenarioBlock { get; internal set; }
        public Exception TestError { get; internal set; }
        public ValueRetrieverCollection ValueRetrievers { get; private set; }

        internal TestStatus TestStatus { get; set; }
        internal List<string> PendingSteps { get; private set; }
        internal List<string> MissingSteps { get; private set; }
        internal Stopwatch Stopwatch { get; private set; }

        internal ITestRunner TestRunner { get; private set; } 

        private readonly IObjectContainer objectContainer;

        internal ScenarioContext(ScenarioInfo scenarioInfo, ITestRunner testRunner, IObjectContainer parentContainer)
        {
            this.objectContainer = parentContainer == null ? new ObjectContainer() : new ObjectContainer(parentContainer);
            TestRunner = testRunner;

            Stopwatch = new Stopwatch();
            Stopwatch.Start();

            CurrentScenarioBlock = ScenarioBlock.None;
            ScenarioInfo = scenarioInfo;
            TestStatus = TestStatus.OK;
            PendingSteps = new List<string>();
            MissingSteps = new List<string>();

            ValueRetrievers = new ValueRetrieverCollection();
            SetDefaultValueRetrievers();
        }

        private void SetDefaultValueRetrievers()
        {
            // Basic types
            ValueRetrievers.Set<bool, BoolValueRetriever>();
            ValueRetrievers.Set<byte, ByteValueRetriever>();
            ValueRetrievers.Set<char, CharValueRetriever>();
            ValueRetrievers.Set<DateTime, DateTimeValueRetriever>();
            ValueRetrievers.Set<decimal, DecimalValueRetriever>();
            ValueRetrievers.Set<double, DoubleValueRetriever>();
            ValueRetrievers.Set<float, FloatValueRetriever>();
            ValueRetrievers.Set<Guid, GuidValueRetriever>();
            ValueRetrievers.Set<int, IntValueRetriever>();
            ValueRetrievers.Set<long, LongValueRetriever>();
            ValueRetrievers.Set<uint, UIntValueRetriever>();
            ValueRetrievers.Set<sbyte, SByteValueRetriever>();
            ValueRetrievers.Set<short, ShortValueRetriever>();
            ValueRetrievers.Set<string, StringValueRetriever>();
            ValueRetrievers.Set<ulong, ULongValueRetriever>();
            ValueRetrievers.Set<ushort, UShortValueRetriever>();

            // Enum
            ValueRetrievers.Set<Enum, EnumValueRetriever>();

            // Nullables
            ValueRetrievers.Set<bool?, NullableBoolValueRetriever>();
            ValueRetrievers.Set<byte?, NullableByteValueRetriever>();
            ValueRetrievers.Set<char?, NullableCharValueRetriever>();
            ValueRetrievers.Set<DateTime?, NullableDateTimeValueRetriever>();
            ValueRetrievers.Set<decimal?, NullableDecimalValueRetriever>();
            ValueRetrievers.Set<double?, NullableDoubleValueRetriever>();
            ValueRetrievers.Set<float?, NullableFloatValueRetriever>();
            ValueRetrievers.Set<Guid?, NullableGuidValueRetriever>();
            ValueRetrievers.Set<int?, NullableIntValueRetriever>();
            ValueRetrievers.Set<long?, NullableLongValueRetriever>();
            ValueRetrievers.Set<uint?, NullableUIntValueRetriever>();
            ValueRetrievers.Set<sbyte?, NullableSByteValueRetriever>();
            ValueRetrievers.Set<short?, NullableShortValueRetriever>();
            ValueRetrievers.Set<ulong?, NullableULongValueRetriever>();
            ValueRetrievers.Set<ushort?, NullableUShortValueRetriever>();
        }

        public void Pending()
        {
            TestRunner.Pending();
        }

        public object GetBindingInstance(Type bindingType)
        {
            return objectContainer.Resolve(bindingType);
        }

        internal void SetBindingInstance(Type bindingType, object instance)
        {
            objectContainer.RegisterInstanceAs(instance, bindingType);
        }

        protected override void Dispose()
        {
            base.Dispose();

            objectContainer.Dispose();
        }
    }
}