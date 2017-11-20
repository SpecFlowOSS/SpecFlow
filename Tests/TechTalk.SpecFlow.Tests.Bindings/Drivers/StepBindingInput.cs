﻿namespace TechTalk.SpecFlow.Tests.Bindings.Drivers
{
    public class StepBindingInput
    {
        public ScenarioBlock ScenarioBlock { get; private set; }
        public string Regex { get; private set; }
        public string Code { get; private set; }
        

        public string ParameterName { get; set; }
        public string ParameterType { get; set; }

        public StepBindingInput(ScenarioBlock scenarioBlock, string regex, string code)
        {
            ScenarioBlock = scenarioBlock;
            Regex = regex;
            Code = code;
        }
    }
}