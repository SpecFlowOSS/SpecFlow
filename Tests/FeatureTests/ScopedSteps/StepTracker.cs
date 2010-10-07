using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace TechTalk.SpecFlow.FeatureTests.ScopedSteps
{
    public class StepTracker
    {
        private readonly List<string> executedSteps = new List<string>();

        public void StepExecuted(string stepText)
        {
            executedSteps.Add(stepText);
        }

        private bool IsStepExecuted(string stepTextPattern)
        {
            Regex stepTextRe = new Regex("^" + stepTextPattern + "$");

            return executedSteps.Any(executedStep => stepTextRe.Match(executedStep).Success);
        }

        public void AssertStepExecuted(string stepTextPattern)
        {
            if (IsStepExecuted(stepTextPattern))
                return;

            Assert.Fail(string.Format("The expected step was not executed: '{0}'", stepTextPattern));
        }

        public void AssertStepNotExecuted(string stepTextPattern)
        {
            if (IsStepExecuted(stepTextPattern))
                Assert.Fail(string.Format("The expected step was executed: '{0}'", stepTextPattern));
        }
    }
}