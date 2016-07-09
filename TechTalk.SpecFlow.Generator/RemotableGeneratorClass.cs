using System;

namespace TechTalk.SpecFlow.Generator
{
    [Serializable]
    public abstract class RemotableGeneratorClass : MarshalByRefObject
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}