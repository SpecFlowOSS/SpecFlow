using System;
using System.Linq;
using BoDi;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class TestObjectResolver : ITestObjectResolver
    {
        public object ResolveBindingInstance(Type bindingType, IObjectContainer container)
        {
            return container.Resolve(bindingType);
        }
    }
}
