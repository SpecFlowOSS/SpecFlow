using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow
{
    internal class Configuration
    {
        static public Configuration Current
        {
            get { return ObjectContainer.Configuration; }
        }

        private const bool StopAtFirstErrorDefault = false;
        private const bool UseIgnoreForMissingOrPendingStepsDefault = false;
        private const bool TraceTimingsDefault = false;
        private const bool TraceSuccessfulStepsDefault = true;
        private const bool DetectAmbiguousMatchesDefault = true;
        private const double MinTracedDurationDefault = 0.1;

        public bool StopAtFirstError { get; set; }
        public bool DetectAmbiguousMatches { get; set; }
        public bool TraceSuccessfulSteps { get; set; }
        public bool TraceTimings { get; set; }
        public bool UseIgnoreForMissingOrPendingSteps { get; set; }
        public TimeSpan MinTracedDuration { get; set; }

        public Configuration()
        {
            MinTracedDuration = TimeSpan.FromSeconds(MinTracedDurationDefault);
            DetectAmbiguousMatches = DetectAmbiguousMatchesDefault;
            TraceSuccessfulSteps = TraceSuccessfulStepsDefault;
            TraceTimings = TraceTimingsDefault;
            UseIgnoreForMissingOrPendingSteps = UseIgnoreForMissingOrPendingStepsDefault;
            StopAtFirstError = StopAtFirstErrorDefault;
        }

        private static bool GetBoolConfig(string name, bool defaultValue)
        {
            string value = ConfigurationManager.AppSettings["SpecFlow." + name];
            if (value == null)
                return defaultValue;
            return bool.Parse(value);
        }

        private static double GetDoubleConfig(string name, double defaultValue)
        {
            string value = ConfigurationManager.AppSettings["SpecFlow." + name];
            if (value == null)
                return defaultValue;
            return double.Parse(value);
        }

        public static Configuration LoadFromConfigFile()
        {
            var config = new Configuration();
            config.MinTracedDuration = TimeSpan.FromSeconds(GetDoubleConfig("MinTracedDuration", MinTracedDurationDefault));
            config.DetectAmbiguousMatches = GetBoolConfig("DetectAmbiguousMatches", DetectAmbiguousMatchesDefault);
            config.TraceSuccessfulSteps = GetBoolConfig("TraceSuccessfulSteps", TraceSuccessfulStepsDefault);
            config.TraceTimings = GetBoolConfig("TraceTimings", TraceTimingsDefault);
            config.UseIgnoreForMissingOrPendingSteps = GetBoolConfig("UseIgnoreForMissingOrPendingSteps", UseIgnoreForMissingOrPendingStepsDefault);
            config.StopAtFirstError = GetBoolConfig("StopAtFirstError", StopAtFirstErrorDefault);
            return config;
        }
    }
}
