using BoDi;
using System;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.Assist
{
    public static class Container
    {
        public static IObjectContainer Setup(IObjectContainer container = null)
        {
            if (container == null)
            {
                container = new ObjectContainer();
                (new DefaultDependencyProvider ()).RegisterDefaults((ObjectContainer)container);
            }
            return container;
        }   
    }
}

