using System;


namespace TechTalk.SpecFlow.Generator
{
    [Serializable]
    public abstract class RemotableGeneratorClass : MarshalByRefObject
    {
#pragma warning disable 672
        public override object InitializeLifetimeService()
#pragma warning restore 672
        {
            return null;
        }
    }
}