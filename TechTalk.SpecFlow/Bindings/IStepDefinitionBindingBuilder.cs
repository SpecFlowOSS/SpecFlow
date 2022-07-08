using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Bindings;

public interface IStepDefinitionBindingBuilder
{
    public IEnumerable<IStepDefinitionBinding> Build();
}