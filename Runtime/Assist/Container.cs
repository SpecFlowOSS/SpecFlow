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

            container.RegisterInstanceAs<IValueComparer>(new DateTimeValueComparer(), "datetime");
            container.RegisterInstanceAs<IValueComparer>(new BoolValueComparer(), "bool");
            container.RegisterInstanceAs<IValueComparer>(new GuidValueComparer(new GuidValueRetriever()), "guid");
            container.RegisterInstanceAs<IValueComparer>(new DecimalValueComparer(), "decimal");
            container.RegisterInstanceAs<IValueComparer>(new DoubleValueComparer(), "double");
            container.RegisterInstanceAs<IValueComparer>(new FloatValueComparer(), "float");
            container.RegisterInstanceAs<IValueComparer>(new DefaultValueComparer(), "default");

            return container;
        }   
    }
}

