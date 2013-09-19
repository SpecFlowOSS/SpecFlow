using System.ComponentModel.Composition;
using System.Linq;
using System;
using BoDi;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    public interface IBiDiContainerProvider
    {
        IObjectContainer ObjectContainer { get; }
    }

    [Export(typeof(IBiDiContainerProvider))]
    internal class BiDiContainerProvider : IBiDiContainerProvider
    {
        public static IObjectContainer CurrentContainer { get; internal set; }

        public IObjectContainer ObjectContainer
        {
            get { return CurrentContainer; }
        }
    }
}