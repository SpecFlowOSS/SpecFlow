﻿using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Generation;
using TechTalk.SpecFlow.Generator.UnitTestConverter;

namespace TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin
{
    public class CombinationFeatureGenerator : UnitTestFeatureGenerator
    {
        
        public CombinationFeatureGenerator(CodeDomHelper codeDomHelper, SpecFlowConfiguration specFlowConfiguration, IDecoratorRegistry decoratorRegistry, Combination combination) :
            base(new CustomXUnitGeneratorProvider(codeDomHelper, combination), codeDomHelper, specFlowConfiguration, decoratorRegistry)
        {
        
        }
    }
}
