using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class StepArgumentTransformations
    {
        [StepArgumentTransformation("once")]
        public int ConvertOnce()
        {
            return 1;
        }

        [StepArgumentTransformation("twice")]
        public int ConvertTwice()
        {
            return 2;
        }

        [StepArgumentTransformation(@"(\d+) times")]
        public int ConvertTimes(int times)
        {
            return times;
        }
    }
}
