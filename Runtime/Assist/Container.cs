using BoDi;
using System;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist
{
    public class Container
    {

        public static Container Instance { get; internal set; }

        static Container()
        {
            Instance = new Container();
        }

        public IObjectContainer Setup(IObjectContainer container = null)
        {
            if (container == null)
            {
                container = new ObjectContainer();
                (new DefaultDependencyProvider ()).RegisterDefaults((ObjectContainer)container);
            }

            Service.Instance.LoadContainer(container);

            return container;
        }   
    }
}

